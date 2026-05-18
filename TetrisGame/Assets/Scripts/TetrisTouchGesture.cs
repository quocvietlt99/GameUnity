using UnityEngine;

public class TetrisTouchGesture : MonoBehaviour
{
    [SerializeField] private RectTransform holdTouchZone;
    [SerializeField] private float minSwipeDistance = 80f;
    [SerializeField] private float tapMaxDistance = 35f;

    private Vector2 touchStartPosition;
    private bool isTracking;

    private void Update()
    {
        if (Input.touchCount == 0)
        {
            return;
        }

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            isTracking = true;
            touchStartPosition = touch.position;
        }

        if (!isTracking)
        {
            return;
        }

        if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            isTracking = false;
            HandleTouchEnd(touch.position);
        }
    }

    private void HandleTouchEnd(Vector2 endPosition)
    {
        Vector2 delta = endPosition - touchStartPosition;

        if (holdTouchZone != null && RectTransformUtility.RectangleContainsScreenPoint(holdTouchZone, endPosition))
        {
            if (Spawner.Instance != null)
            {
                Spawner.Instance.HoldCurrentPiece();
            }

            return;
        }

        if (delta.magnitude < tapMaxDistance)
        {
            HandleTap(endPosition);
            return;
        }

        if (Mathf.Abs(delta.y) > Mathf.Abs(delta.x) && Mathf.Abs(delta.y) >= minSwipeDistance)
        {
            if (delta.y > 0f)
            {
                if (Tetromino.ActiveTetromino != null)
                {
                    Tetromino.ActiveTetromino.HardDrop();
                }
            }
            else
            {
                if (Tetromino.ActiveTetromino != null)
                {
                    Tetromino.ActiveTetromino.SoftDrop();
                }
            }

            return;
        }

        if (Mathf.Abs(delta.x) >= minSwipeDistance)
        {
            if (delta.x > 0f)
            {
                if (Tetromino.ActiveTetromino != null)
                {
                    Tetromino.ActiveTetromino.MoveRight();
                }
            }
            else
            {
                if (Tetromino.ActiveTetromino != null)
                {
                    Tetromino.ActiveTetromino.MoveLeft();
                }
            }
        }
    }

    private void HandleTap(Vector2 position)
    {
        float width = Screen.width;

        if (position.x < width * 0.33f)
        {
            if (Tetromino.ActiveTetromino != null)
            {
                Tetromino.ActiveTetromino.MoveLeft();
            }
        }
        else if (position.x > width * 0.66f)
        {
            if (Tetromino.ActiveTetromino != null)
            {
                Tetromino.ActiveTetromino.MoveRight();
            }
        }
        else
        {
            if (Tetromino.ActiveTetromino != null)
            {
                Tetromino.ActiveTetromino.RotateClockwise();
            }
        }
    }
}