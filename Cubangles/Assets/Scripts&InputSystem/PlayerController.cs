using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class PlayerController : MonoBehaviour ,IDamageable
{
    internal PlayerSendMessages psm;
    private Rigidbody2D rb;


    public int health { get; set; }


    [Header("Player Movement")]
    [SerializeField] private FloatingJoystick movementJoystick;
    [SerializeField] private FloatingJoystick lookJoystick;
    private Vector2 direction;
    private Vector2 rotationDirection;
    [SerializeField] private float moveSpeed;
    [SerializeField] public Camera cam;

    [Header("Player Dash")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashCooldown;
    [SerializeField] private float dashTime = 0.2f;

    [SerializeField] private TextMeshProUGUI dashCoolDownText;
    [SerializeField] private Image dashButtonImage;

    [SerializeField] private Color dashButtonCooldownColor;

    [SerializeField] private TrailRenderer tr;

    private bool canDash = true;
    private bool isDashing;

    

    [Header("Player Heal")]
    [SerializeField] private int healCount;
    [SerializeField] private TextMeshProUGUI healCountText;


    [Header("Player Fire")]
    [SerializeField] private GameObject bulletPrefab;
     
    private Transform bulletAngle;
    [SerializeField] internal GunSO currentGun;
    [SerializeField] private float bulletForce;

    internal bool canAttack = true; 
    [SerializeField] private GameObject gameOverMenu;


    [Header("Player HealthBar")]
    [SerializeField] internal Image[] hearths;
    internal int hearthNumber = -1;


    private float movementAngle;

    bool canCheck = true;

    [HideInInspector] public Vector3 playerStartPos;
    
    void Awake()
    {
        gameOverMenu.SetActive(false);
        bulletAngle = GameObject.Find("BulletAngle").GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        psm = GetComponent<PlayerSendMessages>();
    }
    private void Start()
    {
        canCheck = true;
        health = hearths.Length;
        healCountText.text = healCount.ToString();
        this.gameObject.SetActive(true);
    }
    private void Update()
    {
        direction.x = movementJoystick.GetInput().x;
        direction.y = movementJoystick.GetInput().y;

        rotationDirection.x = lookJoystick.GetInput().x;
        rotationDirection.y = lookJoystick.GetInput().y;

        
    }
    void FixedUpdate()
    {
        if (canCheck)
        {
            playerStartPos = transform.position;
            canCheck = false;
        }
        if (!isDashing)
        {
            rb.velocity = (new Vector2(direction.x * moveSpeed, direction.y * moveSpeed));
        }



        if (rotationDirection.x != 0 || rotationDirection.y != 0)
        {
            movementAngle = Mathf.Atan2(rotationDirection.y, rotationDirection.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = movementAngle;
            Fire();
        }
        else if((direction.x != 0 || direction.y != 0) && rotationDirection.x == 0 && rotationDirection.y == 0)
        {
            movementAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        }
        else
        {
            movementAngle = 0;
        }
        rb.rotation = movementAngle;

    }
    private void LateUpdate()
    {
        cam.transform.position = new Vector3(transform.position.x , transform.position.y , cam.transform.position.z);
    }
    void Fire()
    {
        if (canAttack)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletAngle.position, bulletAngle.rotation);
            bullet.GetComponent<PlayerBulletScript>().damage = currentGun.bulletDamage;

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(bulletAngle.up * bulletForce , ForceMode2D.Impulse);
            StartCoroutine(CanAttackDelay());
        }
    }

    public void Dash()
    {
        if (canDash)
        {
            rb.AddForce(dashSpeed * direction * Time.deltaTime, ForceMode2D.Impulse);
            StartCoroutine(DashEnumerator());
            StartCoroutine(Cooldown(dashCooldown, dashCoolDownText, dashButtonImage, dashButtonCooldownColor, dashButtonImage.color));
        }

    }
    public void Heal(bool isHealthStone)
    {
        if (hearthNumber != -1 && healCount > 0)
        {
            hearths[hearthNumber].gameObject.SetActive(true);
            health++;
            hearthNumber--;

            if (!isHealthStone)
            {
                healCount--;
            }
        }
        else if (isHealthStone)
        {
            healCount++;
        }
        healCountText.text = healCount.ToString();

    }
    IEnumerator Cooldown(float cooldown , TextMeshProUGUI text, Image image , Color cooldownColor, Color nonCooldownColor)
    {
        float knownDashCooldown = cooldown;

        image.color = cooldownColor;
        text.gameObject.SetActive(true);
        text.text = cooldown.ToString();
        yield return new WaitForSeconds(1f);
        cooldown--;
        if (cooldown < 1)
        {
            cooldown = knownDashCooldown;
            text.gameObject.SetActive(false);
            dashButtonImage.color = nonCooldownColor;
            canDash = true;
        }
        else
        {
            StartCoroutine(Cooldown(cooldown, text, image, cooldownColor, nonCooldownColor));
        }
    }
    
    IEnumerator CanAttackDelay()
    {
        canAttack = false;
        yield return new WaitForSeconds(currentGun.attackSpeed);
        canAttack = true;
    }
    public void Damage(int damage)
    {
        health -= damage;
        hearthNumber++;
        hearths[hearthNumber].gameObject.SetActive(false);
        if (health <= 0)
        {
            gameOverMenu.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }
    public void StartGameAgain()
    {
        StopAllCoroutines();

        hearthNumber = -1;
        health = 3;

        canAttack = true;
        canDash = true;

        psm.move = Vector2.zero;
        psm.look = Vector2.zero;

        StartCoroutine(Cooldown(1, dashCoolDownText, dashButtonImage, dashButtonCooldownColor, Color.white));
    }
    private IEnumerator DashEnumerator()
    {
        canDash = false;
        isDashing = true;
        rb.velocity = new Vector2(dashSpeed * direction.x, dashSpeed * direction.y);
        tr.emitting = true;
        yield return new WaitForSeconds(dashTime);
        tr.emitting = false;
        isDashing = false;
    }
}
