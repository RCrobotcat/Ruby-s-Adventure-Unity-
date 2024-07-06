using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RubyController : MonoBehaviour
{
    public float speed = 5.0f;

    public int maxHealth = 5;
    public float timeInvincible = 1.0f; // �޵�ʱ��2��, �޵�ʱ�������Ruby ���ٴ��ܵ��˺�

    public int maxBullet = 20; // ����ӵ�����

    public int bullet { get { return currentBullet; } }
    int currentBullet; // ��ǰ�ӵ�����

    public int health { get { return currentHealth; } }
    int currentHealth;

    public int maxLife = 3;
    public int life { get { return currentLife; } }
    int currentLife;
    public bool isDead;

    bool isInvincible; // defalut false
    float invincibleTimer;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    public GameObject projectilePrefab;
    public ParticleSystem PickEffect;

    public AudioSource audioSourceFootStep; // �Ų�����
    public AudioSource audioSourceOtherClip; // ��������
    public AudioClip hitClip;
    public AudioClip footStep;
    public AudioClip respawnSound;

    private Vector3 respawnPosition; // ����λ��

    public NonPlayerCharacter nearbyNPC;
    public float tipDistance = 1.5f; // �����Ҫ�ӽ�NPC�ľ���
    public bool pressF = false;
    public bool pressF_success = false; // �Ƿ�������ɺ���F��

    public float displayTime = 5.0f;
    float timerDisplay;
    public GameObject tipBox; // ��ʾ��
    public TextMeshProUGUI tipText; // ��ʾ���ı�
    public GameObject success; // �˵��б�
    public GameObject die; // ��������
    public AudioSource BGM;
    // �ڵ�һ��֡����֮ǰ���� Start
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        currentBullet = 5;
        currentLife = maxLife;
        PickEffect.Stop();

        audioSourceFootStep = GetComponents<AudioSource>()[0];
        audioSourceOtherClip = GetComponents<AudioSource>()[1];
        respawnPosition = transform.position; // ��¼����λ��

        UIBullet.instance.bulletUIvisibility(false);
    }

    // ÿ֡����һ�� Update
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f)) // ����������ĳ�������ֵ��Ϊ 0 ʱ��Ruby �����Ÿ÷����ƶ�
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
            if (!audioSourceFootStep.isPlaying)
            {
                audioSourceFootStep.clip = footStep;
                audioSourceFootStep.Play();
            }
        }
        else
        {
            audioSourceFootStep.Stop();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false; // ȡ���޵�״̬
        }

        if (Input.GetKeyDown(KeyCode.E)) // ���� E ��ʱ��Ruby ������һ���ɵ�
        {
            if (UIHealthBar.instance.fixedNum >= 12 && pressF_success)
            {
                success.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None; // ��ʾ���
                Time.timeScale = (0); // ��ͣ��Ϸ
            }
            else
            {
                Launch();
            }
        }

        if (currentLife <= 0)
        {
            displayDie(); // ��ʾ��������
            currentLife = maxLife;
        }

        // ��������NPC�ľ���
        if (nearbyNPC != null)
        {
            float distance = Vector3.Distance(transform.position, nearbyNPC.transform.position);
            if (distance <= tipDistance && !pressF)
            {
                nearbyNPC.DisplayTip();  // �������㹻�ӽ�����ʾ��ʾ��
            }
            else
            {
                nearbyNPC.HideTip();  // �������뿪��������ʾ��
            }
        }

        if (Input.GetKeyDown(KeyCode.F)) // ���� F ��ʱ��Ruby ���� NPC �Ի�
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                // Debug.Log("Raycast has hit the object " + hit.collider.gameObject);
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                    displayTip();
                    pressF = true;
                }
            }
        }

        if (timerDisplay >= 0)
        {
            timerDisplay -= Time.deltaTime;
            if (timerDisplay < 0)
            {
                tipBox.SetActive(false);
            }
        }
    }

        void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void displayTip()
    {
        timerDisplay = displayTime;
        tipBox.SetActive(true);
        if (UIHealthBar.instance.fixedNum >= 12 && pressF_success)
        {
            // �������
            tipText.text = "Press \"E\" To Restart or Quit";
        }
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;

            animator.SetTrigger("Hit");
            PlaySound(hitClip);
            isInvincible = true;
            invincibleTimer = timeInvincible;
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        if(amount > 0)
        {
            PickEffect.Play();
        }
        /*
         ǯ�ƹ��� (Clamping) ��ȷ����һ���������˴�Ϊ currentHealth + amount��������С�ڵڶ����������˴�Ϊ 0����
         Ҳ��������ڵ��������� (maxHealth)����ˣ�Ruby ������ֵ��ʼ�ձ����� 0 �� maxHealth ֮�䡣
         */
        // Debug.Log(currentHealth + "/" + maxHealth);

        if(currentHealth <= 0)
        {
            ChangeLife(-1);
            if(currentLife > 0)
            {
                Respawn();
            }
        }

        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth); // ��������ֵ��
    }

    public void ChangeBullet(int amount)
    {
        currentBullet = Mathf.Clamp(currentBullet + amount, 0, maxBullet);
        if(amount > 0)
        {
            PickEffect.Play();
        }
    }

    public void ChangeLife(int amount)
    {
        currentLife = Mathf.Clamp(currentLife + amount, 0, maxLife);
    }

    void Launch()
    {
        if(!UIHealthBar.instance.hasTask || currentBullet == 0)
        {
            return; // ���û����������ӵ�����Ϊ 0��Ruby ���޷�����ɵ�
        }
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>(); // ��ȡ Projectile ���
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");
        ChangeBullet(-1);
    }

    public void PlaySound(AudioClip clip)
    {
        audioSourceOtherClip.PlayOneShot(clip);
    }

    public void displayDie()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None; // ��ʾ���
        isDead = true;
        die.SetActive(true);
        Time.timeScale = (0); // ��ͣ��Ϸ
        BGM.Pause();
    }

    private void Respawn() // ����
    {
        ChangeHealth(maxHealth);
        transform.position = respawnPosition;
        PlaySound(respawnSound);
    }
}
