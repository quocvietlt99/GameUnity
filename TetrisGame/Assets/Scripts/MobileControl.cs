using UnityEngine;

public class MobileControl : MonoBehaviour
{
    public void MoveLeft()
    {
        if (Tetromino.ActiveTetromino != null)
        {
            Tetromino.ActiveTetromino.MoveLeft();
        }
    }

    public void MoveRight()
    {
        if (Tetromino.ActiveTetromino != null)
        {
            Tetromino.ActiveTetromino.MoveRight();
        }
    }

    public void MoveDown()
    {
        SoftDrop();
    }

    public void MoveUp()
    {
        Rotate();
    }

    public void SoftDrop()
    {
        if (Tetromino.ActiveTetromino != null)
        {
            Tetromino.ActiveTetromino.SoftDrop();
        }
    }

    public void HardDrop()
    {
        if (Tetromino.ActiveTetromino != null)
        {
            Tetromino.ActiveTetromino.HardDrop();
        }
    }

    public void Rotate()
    {
        if (Tetromino.ActiveTetromino != null)
        {
            Tetromino.ActiveTetromino.RotateClockwise();
        }
    }

    public void RotateCounterClockwise()
    {
        if (Tetromino.ActiveTetromino != null)
        {
            Tetromino.ActiveTetromino.RotateCounterClockwise();
        }
    }

    public void Hold()
    {
        if (Spawner.Instance != null)
        {
            Spawner.Instance.HoldCurrentPiece();
        }
    }

    public void Restart()
    {
        if (TetrisGameManager.Instance != null)
        {
            TetrisGameManager.Instance.RestartGame();
        }
    }
}