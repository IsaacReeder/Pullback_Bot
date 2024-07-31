using System;
using cAlgo.API;
using cAlgo.API.Internals;

namespace cAlgo.Robots
{
    [Robot(AccessRights = AccessRights.None, AddIndicators = true)]
    public class cbot_pullback : Robot
    {
        [Parameter("Volume (in units)", DefaultValue = 0.01, MinValue = 0.01, Step = 0.01)]
        public double Volume { get; set; }

        [Parameter("Take Profit (pips)", DefaultValue = 10, MinValue = 1)]
        public int TakeProfitPips { get; set; }

        [Parameter("Stop Loss (pips)", DefaultValue = 10, MinValue = 1)]
        public int StopLossPips { get; set; }

        [Parameter("Risk Percentage", DefaultValue = 1, MinValue = 0.1, MaxValue = 10, Step = 0.1)]
        public double RiskPercentage { get; set; }

        [Parameter("Fast EMA Period", DefaultValue = 9, MinValue = 1)]
        public int FastEmaPeriod { get; set; }

        [Parameter("Slow EMA Period", DefaultValue = 21, MinValue = 1)]
        public int SlowEmaPeriod { get; set; }

        private IndicatorDataSeries fastEma;
        private IndicatorDataSeries slowEma;
        private double stopLossDistance;
        private double takeProfitDistance;

        protected override void OnStart()
        {
            // Initialize EMAs
            fastEma = Indicators.ExponentialMovingAverage(Bars.ClosePrices, FastEmaPeriod).Result;
            slowEma = Indicators.ExponentialMovingAverage(Bars.ClosePrices, SlowEmaPeriod).Result;

            Print("Pullback Strategy Started");
        }

        protected override void OnTick()
        {
            // Handle price updates here if needed
        }

        protected override void OnBar()
        {
            // Calculate dynamic StopLoss and TakeProfit in terms of pips
            stopLossDistance = StopLossPips * Symbol.PipSize;
            takeProfitDistance = TakeProfitPips * Symbol.PipSize;

            // Ensure we have enough bars for our strategy
            if (Bars.Count < Math.Max(FastEmaPeriod, SlowEmaPeriod) + 1)
                return;

            // Determine current trend based on EMAs
            bool isBullish = fastEma.Last(1) > slowEma.Last(1);
            bool isBearish = fastEma.Last(1) < slowEma.Last(1);

            // Pullback buying condition
            if (isBullish && Bars.ClosePrices.Last(1) < fastEma.Last(1) && Bars.ClosePrices.Last(1) < Bars.OpenPrices.Last(1))
            {
                Print("Pullback Buy Signal Detected");

                if (IsTradeAllowed(TradeType.Buy))
                {
                    ExecuteMarketOrder(TradeType.Buy, Symbol.Name, CalculateVolume(), "Pullback Buy", stopLossDistance, takeProfitDistance);
                }
            }

            // Pullback selling condition
            if (isBearish && Bars.ClosePrices.Last(1) > fastEma.Last(1) && Bars.ClosePrices.Last(1) > Bars.OpenPrices.Last(1))
            {
                Print("Pullback Sell Signal Detected");

                if (IsTradeAllowed(TradeType.Sell))
                {
                    ExecuteMarketOrder(TradeType.Sell, Symbol.Name, CalculateVolume(), "Pullback Sell", stopLossDistance, takeProfitDistance);
                }
            }
        }

        private bool IsTradeAllowed(TradeType tradeType)
        {
            // Check if there's an open position already
            foreach (var position in Positions)
            {
                if (position.SymbolName == Symbol.Name && position.TradeType == tradeType)
                {
                    Print("Position already open: {0}", position.TradeType);
                    return false;
                }
            }

            return true;
        }

        private double CalculateVolume()
        {
            // Calculate volume based on risk percentage and stop loss distance
            double accountRisk = Account.Balance * (RiskPercentage / 100);
            double riskPerTrade = stopLossDistance * Symbol.PipValue * Volume;
            return Math.Min(accountRisk / riskPerTrade, Volume);
        }

        protected override void OnStop()
        {
            // Handle cBot stop here
        }
    }
}
