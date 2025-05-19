using Microsoft.Xna.Framework.Input;
using Murder.Utilities;

namespace Murder.Core.Input;

public class HapticsManager
{
    private const int MAX_CONTROLLERS = 4;

    private readonly List<HapticsOrder> _currentOrders = new List<HapticsOrder>(128);
    private readonly float[] _leftVibration = new float[MAX_CONTROLLERS];
    private readonly float[] _rightVibration = new float[MAX_CONTROLLERS];

    public void Update()
    {
        for (int i = 0; i < MAX_CONTROLLERS; i++)
        {
            _leftVibration[i] = 0;
            _rightVibration[i] = 0;
        }

        for (int i = _currentOrders.Count - 1; i >= 0; i--)
        {
            var order = _currentOrders[i];

            if (!GamePad.GetState(order.Player).IsConnected)
                continue;

            var delta = Calculator.ClampTime(order.StartTime, Game.NowUnscaled, order.EndTime - order.StartTime);

            if (delta >= 1)
            {
                _currentOrders.RemoveAt(i);
            }
            else
            {
                int playerIndex = (int)order.Player;
                float currentLeft = _leftVibration[playerIndex];
                float currentRight = _rightVibration[playerIndex];

                float currentVibration = Calculator.Curve(delta, order.StartVibration, order.MiddleVibration, order.EndVibration);
                float currentBalance = Calculator.Curve(delta, order.BalanceStart, order.BalanceMiddle, order.BalanceEnd);

                float leftVibration = currentVibration * currentBalance * 2;
                float rightVibration = currentVibration * (1 - currentBalance) * 2;

                _leftVibration[playerIndex] = Math.Max(leftVibration, currentLeft);
                _rightVibration[playerIndex] = Math.Max(rightVibration, currentRight);
            }
        }

        for (int i = 0; i < MAX_CONTROLLERS; i++)
        {
            if (!GamePad.GetState((Microsoft.Xna.Framework.PlayerIndex)i).IsConnected)
            {
                continue;
            }

            GamePad.SetVibration((Microsoft.Xna.Framework.PlayerIndex)i, _leftVibration[i], _rightVibration[i]);
        }
    }

    public void Add(HapticsOrder order)
    {
        _currentOrders.Add(order);
    }

    internal void ClearVibration(Microsoft.Xna.Framework.PlayerIndex player)
    {
        for (int i = _currentOrders.Count - 1; i >= 0; i--)
        {
            if (_currentOrders[i].Player == player)
            {
                _currentOrders.RemoveAt(i);
            }
        }
    }

    public void ClearAll()
    {
        for (int i = 0; i < MAX_CONTROLLERS; i++)
        {
            _leftVibration[i] = 0;
            _rightVibration[i] = 0;

            if (!GamePad.GetState((Microsoft.Xna.Framework.PlayerIndex)i).IsConnected)
            {
                continue;
            }

            GamePad.SetVibration((Microsoft.Xna.Framework.PlayerIndex)i, 0, 0);
        }
    }
}

public readonly struct HapticsOrder
{
    public readonly Microsoft.Xna.Framework.PlayerIndex Player { get; init; } = Microsoft.Xna.Framework.PlayerIndex.One;
    public readonly float StartVibration { get; init; } = 0;
    public readonly float MiddleVibration { get; init; } = 0;
    public readonly float EndVibration { get; init; } = 0;

    public readonly float BalanceStart { get; init; } = 0.5f;
    public readonly float BalanceMiddle { get; init; } = 0.5f;
    public readonly float BalanceEnd { get; init; } = 0.5f;

    public readonly float StartTime { get; init; } = 0;
    public readonly float EndTime { get; init; } = 0;

    public HapticsOrder()
    {

    }
}
