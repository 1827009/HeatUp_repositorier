using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
	[Header("HpOptins")]
	[TooltipAttribute("キャラクターの体力")]
	public int maxHp = 100;

	[TooltipAttribute("キャラクターが耐えられる温度")]
	public int maxTemperature = 1000;

	[TooltipAttribute("ダメージ後何秒で回復を始めるか")]
	public float recoveryTime = 5;

	[TooltipAttribute("何秒に一度回復するか")]
	public float recoverInterval = 0.1f;

	[TooltipAttribute("回復するHP")]
	public int recoveryHp = 1;

	[TooltipAttribute("HPゲージの取得")]
	public TemperatureGage temperatureGage;

	[Header("SpeedOptions")]

	[TooltipAttribute("キャラクターの速度")]
	public float moveSpeed = 6;

	[TooltipAttribute("空中にいる時の加速時間")]
	public float accelerationTimeAirborne = .2f;

	[TooltipAttribute("着地しているときの加速時間")]
	public float accelerationTimeGrounded = .1f;

	[TooltipAttribute("インプットを停止させておくか")]
	public bool defaultInputStop = false;

	[Header("JumpOptions")]
	[TooltipAttribute("ジャンプの高さ")]
	public float jumpHeight = 4;

	[TooltipAttribute("ジャンプの滞空時間")]
	public float timeToJumpApex = .4f;

	[Header("ClimbOptions")]
	[TooltipAttribute("登る速度")]
	public float climbSpeed = 2;

	[Header("AttackOptions")]
	[TooltipAttribute("下に樽があるかどうかを調べる距離")]
	public float findDistance = 1;

	[HideInInspector]
	public int hp;

	Spine.Unity.SkeletonAnimation skeletonAnimation;
	Animator animator;
	[HideInInspector]
	public Controller2D controller;
	CameraController cameraController;

	void Start()
	{
		skeletonAnimation = GetComponent<Spine.Unity.SkeletonAnimation>();
		animator = GetComponent<Animator>();
		controller = GetComponent<Controller2D>();
		sword = GetComponent<Sword>();
		cameraController = Camera.main.GetComponent<CameraController>();

		gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

		right = true;
		tow_jump = false;
		hp = maxHp;
		inputStop = defaultInputStop;

		shadow = transform.GetChild(3).GetComponent<SpriteRenderer>();
		defAlpha = shadow.color.a;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			ApplyDamage(maxHp);
		}
		if (controller.collisions.above || controller.collisions.below)
		{
			velocity.y = 0;
		}

		input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		if (Input.GetKey(KeyCode.Escape))
		{
			Quit();
		}
		CompulsionMove(ref input);
		Move(input);
		Jump();
		Attack();
		FindToggleCollision();
		Recovary();
		controller.Move(velocity * Time.deltaTime);
	}

	#region Move

	Vector3 velocity;
	float velocityXSmoothing;
	[HideInInspector]
	public bool right = true;
	[HideInInspector]
	public bool inputStop = false;
	[HideInInspector]
	public Vector2 compulsionInput = Vector3.zero;

	Vector2 input;

	void Move(Vector2 input)
	{
		if (0 != input.x)
		{
			right = (0 < input.x);
			animator.SetBool("isWalk", true);
		}
		else
		{
			animator.SetBool("isWalk", false);
		}
		skeletonAnimation.skeleton.FlipX = !right;
		float targetVelocityX = input.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
	}

	#endregion

	#region Jump

	float gravity;
	float jumpVelocity;
	[HideInInspector]
	public bool ignoreGravity = false;
	[HideInInspector]
	public bool tow_jump;

	void Jump()
	{
		if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown("joystick button 0")) && controller.IsGrounded)
		{
			tow_jump = false;
			velocity.y = jumpVelocity;
		}
		ClimbInput();
		if (!ignoreGravity)
		{
			velocity.y += gravity * Time.deltaTime;
		}
		if (controller.IsGrounded && !oldIsGrounded) 
		{
			StartCoroutine(Shadow(true));
		}
		if (!controller.IsGrounded && oldIsGrounded) 
		{
			StartCoroutine(Shadow(false));
		}
		oldIsGrounded = controller.IsGrounded;
	}

	public void TaruJump(float x)
	{
		velocity.y = jumpVelocity * x;
		velocity.x = jumpVelocity * x * input.x;
	}

	#endregion

	#region Climb

	[HideInInspector]
	public bool climb = false;

	void ClimbInput()
	{
		if (climb)
		{
			float axis = Input.GetAxis("Vertical");
			if (axis > 0)
			{
				velocity.y = climbSpeed;
			}
			if (axis < 0)
			{
				velocity.y = -climbSpeed;
			}
			if (axis == 0)
			{
				velocity.y = 0;
			}
		}
	}

	public void Climb()
	{
		ignoreGravity = true;
		velocity.y = 0;
		climb = true;
	}

	public void ResetClimb()
	{
		ignoreGravity = false;
		climb = false;
	}

	#endregion

	#region Attack

	Sword sword;
	private bool attack = false;
	void Attack()
	{
		if (Input.GetKeyDown(KeyCode.Space) || Input.GetAxis("RightTrigger") == 1 && !attack)
		{
			attack = true;
			SwitchAttack();
		}
		if (Input.GetKeyUp(KeyCode.Space) || Input.GetAxis("RightTrigger") != 1 && attack)
		{
			attack = false;
			sword.AttackFinish();
			animator.SetBool("isUnderAttack", false);
			animator.SetBool("isAttack", false);
		}
	}

	void SwitchAttack()
	{
		if (FindBarrels())
		{
			animator.SetBool("isUnderAttack", true);
			sword.Attack(true, controller.collisionMask);
		}
		else
		{
			animator.SetBool("isAttack", true);
			sword.Attack(false, controller.collisionMask);
		}
	}

	bool FindBarrels()
	{
		RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, findDistance, controller.collisionMask);
		if (hit)
		{
			Barrels barrels = hit.collider.GetComponent<Barrels>();
			if (barrels)
			{
				return true;
			}
		}
		return false;
	}

	#endregion

	#region FindToggleCollision

	void FindToggleCollision()
	{
		if (inputStop)
		{
			return;
		}
		Vector2 size = controller.collider.bounds.size;
		Vector3 center = transform.position + new Vector3(0, size.y, 0);
		RaycastHit2D hit = Physics2D.BoxCast(center, size, 0, Vector2.zero, 0, controller.collisionMask);
		if (hit)
		{
			CameraTrigger cameraTrigger = hit.collider.gameObject.GetComponent<CameraTrigger>();
			if (cameraTrigger)
			{
				cameraController.SetCameraTrigger(cameraTrigger);
			}
		}
	}

	void CompulsionMove(ref Vector2 input)
	{
		if (inputStop)
		{
			input = compulsionInput;
		}
	}

	public void SetCollisionMask(LayerMask layerMask)
	{
		if (controller.collisionMask != layerMask)
		{
			controller.collisionMask = layerMask;
		}
	}


	#endregion

	#region Damage

	public void ApplyDamage(int damage)
	{
		timeWithoutDamage = 0;
		hp -= damage;
		if (hp <= 0)
		{

		}
	}

	#endregion

	#region Layer

	public bool CompareLayerAndColliderMask(int layer)
	{
		string str = LayerMask.LayerToName(layer);
		LayerMask layerMask = LayerMask.GetMask(str);
		return controller.collisionMask.value == layerMask.value;
	}

	#endregion

	#region Recovery

	private float timeWithoutDamage = 0;
	private float recovaryCurrentTime = 0;

	private void Recovary()
	{
		timeWithoutDamage += Time.deltaTime;
		if (timeWithoutDamage > recoveryTime)
		{
			temperatureGage.glowing = true;
			recovaryCurrentTime += Time.deltaTime;
			if (recovaryCurrentTime > recoverInterval)
			{
				recovaryCurrentTime = 0;
				hp += recoveryHp;
				hp = Mathf.Clamp(hp, 0, maxHp);
			}
		}
		else
		{
			temperatureGage.glowing = false;
		}
	}

	#endregion

	#region Shadow

	private SpriteRenderer shadow;
	private float shadowSpeed = 5;
	private float defAlpha;
	private bool oldIsGrounded;

	private IEnumerator Shadow(bool fadeIn)
	{
		float alpha = defAlpha;
		while (alpha > 0)
		{
			alpha -= shadowSpeed * Time.deltaTime;
			Color color = shadow.color;
			color.a = fadeIn ? defAlpha - alpha : alpha;
			shadow.color = color;
			yield return null;
		}
		yield break;
	}

	#endregion

	void Quit()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
      UnityEngine.Application.Quit();
#endif
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Vector2 to = transform.position;
		to += Vector2.down * findDistance;
		Gizmos.DrawLine(transform.position, to);
	}
}