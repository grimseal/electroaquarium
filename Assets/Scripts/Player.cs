using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : Character
{
    [SerializeField] private float speed = 0.3f;
    
    private Rigidbody2D rb;

    private float attackStartTime;
    public float attackChargeTime;
    public float invulnerableTimeAfterHit = 0.5f;
    private Vector3 directionVector;

    [SerializeField] private Transform attackContainer;
    [SerializeField] private SpriteRenderer attackSprite;
    [SerializeField] private RangeHandler attackRange;
    private List<Enemy.Enemy> enemiesInAttackRange = new List<Enemy.Enemy>();

    private bool chargeAttack;
    private float chargeProgress;
    private float invulnerableTime;

    private bool hasPushForce;
    private Vector3 pushForce;

    [SerializeField] private RangeHandler grabItemRange;
    [SerializeField] private GameObject grabTip;

    private List<Item> itemsInRange = new List<Item>();
    private List<PowerLine> powerLinesInRange = new List<PowerLine>();
    private List<Equipment> equipments = new List<Equipment>();
    private Item itemInHands;
    private PowerLine powerLineInHands;

    [SerializeField] private Transform itemsContainer;

    protected void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        attackRange.onEnter.AddListener(EnemyEnter);
        attackRange.onExit.AddListener(EnemyLeave);
        grabItemRange.onEnter.AddListener(ItemEnter);
        grabItemRange.onExit.AddListener(ItemLeave);
    }

    private void Update()
    {
        var inputUp = Input.GetKey(KeyCode.W);
        var inputDown = Input.GetKey(KeyCode.S);
        var inputLeft = Input.GetKey(KeyCode.A);
        var inputRight = Input.GetKey(KeyCode.D);

        if (inputUp || inputDown || inputLeft || inputRight)
        {
            directionVector = Vector3.zero;
            if (inputUp) directionVector += Vector3.up;
            if (inputDown) directionVector += Vector3.down;
            if (inputLeft) directionVector += Vector3.left;
            if (inputRight) directionVector += Vector3.right;
            directionVector = directionVector.normalized;
            rb.AddForce(directionVector * speed);

            var a = Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg;
            attackContainer.rotation = Quaternion.Euler(0, 0, a);
        }

        if (Input.GetKeyDown(KeyCode.Space)) AttackCharge();
        if (Input.GetKeyUp(KeyCode.Space)) AttackRelease();

        if (Input.GetKeyDown(KeyCode.E)) GrabItem();

        if (chargeAttack)
        {
            var progress = Mathf.Clamp01((Time.time - attackStartTime) / attackChargeTime);
            attackSprite.color = SetAlpha(attackSprite.color, progress * progress * progress);
        }

        if (hasPushForce)
        {
            if (pushForce.magnitude <= 0.01f) hasPushForce = false;
            rb.AddForce(pushForce);
            pushForce *= 0.5f;
        }
    }

    private void GrabItem()
    {
        var line = GetClosestPowerLine();
        if (line != null)
        {
            GrabCable(line);
            return;
        }
        
        var item = GetClosestItem();
        if (item == null) return;
        item.EnableCollider(false);
        var itemTransform = item.transform;
        itemTransform.SetParent(itemsContainer, false);
        itemTransform.localPosition = Vector3.zero;
        item.gameObject.SetActive(false);
        switch (item.type)
        {
            case Item.Type.Equipment when item is Equipment equip:
                Equip(equip);
                break;
            case Item.Type.Weapon:
                GrabWeapon(item);
                break;
        }
    }

    private void GrabCable(PowerLine targetLine)
    {
        if (powerLineInHands != null && targetLine != powerLineInHands)
        {
            GameManager.Instance.PowerLineJoined(powerLineInHands, targetLine);
            powerLineInHands = null;
            return;
        }
        
        DropItem();
        powerLineInHands = targetLine;
        powerLineInHands.TakeWireBy(transform);
    }

    private void GrabWeapon(Item item)
    {
        DropItem();
        itemInHands = item;
    }

    public void DropItem()
    {
        if (powerLineInHands != null)
        {
            powerLineInHands.Rewind();
            powerLineInHands = null;
            return;
        }
        
        if (itemInHands == null) return;
        if (itemInHands.type == Item.Type.Weapon) BrakeAttack();
        
        itemInHands.gameObject.SetActive(true);
        itemInHands.transform.SetParent(null);
        itemInHands.Push(Random.insideUnitCircle.normalized);
    } 

    private void Equip(Equipment equipment)
    {
        equipments.Add(equipment);
        IncreaseMaxPower();
        switch (equipment.equipmentType)
        {
            case Equipment.EquipmentType.Fins:
                // todo display fins
                return;
            case Equipment.EquipmentType.Hat:
                // todo display hat
                return;
            case Equipment.EquipmentType.Tail:
                // todo display tail
                return;
        }
    }

    private void IncreaseMaxPower()
    {
        powerMax += 1;
        overloaded = false;
        powered = power == powerMax;
    }

    public Item GetClosestItem()
    {
        if (itemsInRange.Count < 1) return null;
        var pos = transform.position;
        var dist = (pos - itemsInRange[0].transform.position).sqrMagnitude;
        var closestItem = itemsInRange[0];
        for (var i = 1; i < itemsInRange.Count; i++)
        {
            var newDist = (pos - itemsInRange[0].transform.position).sqrMagnitude;
            if (dist < newDist) continue;
            dist = newDist;
            closestItem = itemsInRange[0];
        }
        return closestItem;
    }
    
    private List<PowerLine> filteredPowerLinesInRange => powerLinesInRange.Where(l => l != powerLineInHands).ToList();

    public PowerLine GetClosestPowerLine()
    {
        var lines = filteredPowerLinesInRange;
        if (lines.Count < 1) return null;
        var pos = transform.position;
        var dist = (pos - lines[0].transform.position).sqrMagnitude;
        var closestItem = lines[0];
        for (var i = 1; i < lines.Count; i++)
        {
            var newDist = (pos - lines[0].transform.position).sqrMagnitude;
            if (dist < newDist) continue;
            dist = newDist;
            closestItem = lines[0];
        }
        return closestItem;
    }

    private void AttackCharge()
    {
        if (itemInHands == null || itemInHands.type != Item.Type.Weapon) return;
        if (power < 1) return;
        chargeAttack = true;
        attackStartTime = Time.time;
    }

    private void AttackRelease()
    {
        if (!chargeAttack) return;
        chargeAttack = false;
        attackSprite.color = SetAlpha(attackSprite.color, 0);
        if (attackStartTime + attackChargeTime > Time.time) return;
        var enemies = enemiesInAttackRange.Where(enemy => !enemy.overloaded).ToArray();
        if (enemies.Length < 1) return;
        DecreasePower();
        foreach (var enemy in enemies) enemy.HitHandler();
    }

    private void BrakeAttack()
    {
        chargeAttack = false;
        attackSprite.color = SetAlpha(attackSprite.color, 0);
    }

    private void EnemyEnter(Collider2D enemyCollider)
    {
        var enemy = enemyCollider.GetComponent<Enemy.Enemy>();
        if (enemy) enemiesInAttackRange.Add(enemy);
    }

    private void EnemyLeave(Collider2D enemyCollider)
    {
        var enemy = enemyCollider.GetComponent<Enemy.Enemy>();
        if (enemy) enemiesInAttackRange.Remove(enemy);
    }

    public bool grabTipVisible => itemsInRange.Count > 0 || filteredPowerLinesInRange.Count > 0; 
    private void ItemEnter(Collider2D itemCollider)
    {
        var item = itemCollider.GetComponent<Item>();
        if (item) itemsInRange.Add(item);
        var powerLine = itemCollider.GetComponent<PowerLine>();
        if (powerLine && powerLine.canHandle) powerLinesInRange.Add(powerLine);
        grabTip.SetActive(grabTipVisible);
    }

    private void ItemLeave(Collider2D itemCollider)
    {
        var item = itemCollider.GetComponent<Item>();
        if (item) itemsInRange.Remove(item);
        var powerLine = itemCollider.GetComponent<PowerLine>();
        if (powerLine) powerLinesInRange.Remove(powerLine);
        grabTip.SetActive(grabTipVisible);
    }

    public bool HitHandler(int damage)
    {
        if (invulnerableTime > Time.time) return false;
        invulnerableTime = Time.time + invulnerableTimeAfterHit; 
        IncreasePower(damage);
        // todo player overload
        return true;
    }

    private Color SetAlpha(Color color, float a)
    {
        color.a = a;
        return color;
    }

    public override void SetOverloadState()
    {
        base.SetOverloadState();
        
        Debug.Log("GameOver");
    }

    public void Push(Enemy.Enemy by, float force)
    {
        hasPushForce = true;
        pushForce = (transform.position - by.transform.position).normalized * force;
    }
}