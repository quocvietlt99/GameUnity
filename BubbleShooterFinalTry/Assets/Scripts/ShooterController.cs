using UnityEngine;

public class ShooterController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Camera mainCam;
    [SerializeField] private Transform rotatePivot;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bubblePrefab;
    [SerializeField] private SpriteRenderer currentBubbleVisual;
    [SerializeField] private SpriteRenderer nextBubbleVisual;
    [SerializeField] private TrajectoryPreview trajectoryPreview;

    [Header("Aim")]
    [SerializeField] private float minAngle = 25f;
    [SerializeField] private float maxAngle = 155f;

    [Header("Shooter Position")]
    [SerializeField] private Vector3 shooterWorldPosition = new Vector3(0f, -4.3f, 0f);

    private BubbleColor currentColor;
    private BubbleColor nextColor;
    private int activeColorCount = 4;
    private bool canShoot = false;
    private Vector2 currentShootDirection = Vector2.up;

    public void Init(int colorCount)
    {
        activeColorCount = Mathf.Clamp(colorCount, 3, 5);

        currentColor = GetRandomColor();
        nextColor = GetRandomColor();

        transform.position = shooterWorldPosition;
        RefreshVisuals();

        canShoot = true;
    }

    private void Update()
    {
        if (!canShoot) return;
        if (mainCam == null || rotatePivot == null || firePoint == null) return;

        Vector3 mouseScreen = Input.mousePosition;
        Vector3 mouseWorld = mainCam.ScreenToWorldPoint(
            new Vector3(mouseScreen.x, mouseScreen.y, -mainCam.transform.position.z)
        );

        Vector2 dir = mouseWorld - rotatePivot.position;

        // Không cho ngắm xuống dưới
        if (dir.y < 0f)
            dir.y = 0.01f;

        if (dir.sqrMagnitude < 0.001f)
            dir = Vector2.up;

        float rawAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float clampedAngle = Mathf.Clamp(rawAngle, minAngle, maxAngle);

        rotatePivot.rotation = Quaternion.Euler(0f, 0f, clampedAngle - 90f);
        currentShootDirection = firePoint.up.normalized;

        if (trajectoryPreview != null)
        {
            trajectoryPreview.Draw(firePoint.position, currentShootDirection);
        }

        if (Input.GetMouseButtonUp(0))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (!canShoot) return;
        if (bubblePrefab == null) return;

        GameObject obj = Instantiate(bubblePrefab, firePoint.position, Quaternion.identity);
        Bubble bubble = obj.GetComponent<Bubble>();
        BubbleProjectile projectile = obj.GetComponent<BubbleProjectile>();

        if (bubble == null || projectile == null)
        {
            Destroy(obj);
            return;
        }

        bubble.SetColor(currentColor);
        projectile.Launch(currentShootDirection);

        canShoot = false;

        if (currentBubbleVisual != null)
            currentBubbleVisual.enabled = false;

        GameManager.Instance.UseShot();
    }

    public void ReloadAfterShot()
    {
        currentColor = nextColor;
        nextColor = GetRandomColor();

        RefreshVisuals();

        if (currentBubbleVisual != null)
            currentBubbleVisual.enabled = true;

        canShoot = true;
    }

    private void RefreshVisuals()
    {
        if (currentBubbleVisual != null)
        {
            currentBubbleVisual.enabled = true;
            currentBubbleVisual.color = Bubble.GetUnityColor(currentColor);
        }

        if (nextBubbleVisual != null)
        {
            nextBubbleVisual.enabled = true;
            nextBubbleVisual.color = Bubble.GetUnityColor(nextColor);
        }
    }

    private BubbleColor GetRandomColor()
    {
        int value = Random.Range(0, activeColorCount);
        return (BubbleColor)value;
    }
}