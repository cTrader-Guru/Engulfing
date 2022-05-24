/*  CTRADER GURU

    Homepage    : https://ctrader.guru/
    Telegram    : https://t.me/ctraderguru
    Twitter     : https://twitter.com/cTraderGURU/
    Facebook    : https://www.facebook.com/ctrader.guru/
    YouTube     : https://www.youtube.com/channel/UCKkgbw09Fifj65W5t5lHeCQ
    GitHub      : https://github.com/ctrader-guru

*/

using System;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;

namespace cAlgo
{

    public static class Extensions
    {

        #region Enum

        public enum ColorNameEnum
        {

            AliceBlue,
            AntiqueWhite,
            Aqua,
            Aquamarine,
            Azure,
            Beige,
            Bisque,
            Black,
            BlanchedAlmond,
            Blue,
            BlueViolet,
            Brown,
            BurlyWood,
            CadetBlue,
            Chartreuse,
            Chocolate,
            Coral,
            CornflowerBlue,
            Cornsilk,
            Crimson,
            Cyan,
            DarkBlue,
            DarkCyan,
            DarkGoldenrod,
            DarkGray,
            DarkGreen,
            DarkKhaki,
            DarkMagenta,
            DarkOliveGreen,
            DarkOrange,
            DarkOrchid,
            DarkRed,
            DarkSalmon,
            DarkSeaGreen,
            DarkSlateBlue,
            DarkSlateGray,
            DarkTurquoise,
            DarkViolet,
            DeepPink,
            DeepSkyBlue,
            DimGray,
            DodgerBlue,
            Firebrick,
            FloralWhite,
            ForestGreen,
            Fuchsia,
            Gainsboro,
            GhostWhite,
            Gold,
            Goldenrod,
            Gray,
            Green,
            GreenYellow,
            Honeydew,
            HotPink,
            IndianRed,
            Indigo,
            Ivory,
            Khaki,
            Lavender,
            LavenderBlush,
            LawnGreen,
            LemonChiffon,
            LightBlue,
            LightCoral,
            LightCyan,
            LightGoldenrodYellow,
            LightGray,
            LightGreen,
            LightPink,
            LightSalmon,
            LightSeaGreen,
            LightSkyBlue,
            LightSlateGray,
            LightSteelBlue,
            LightYellow,
            Lime,
            LimeGreen,
            Linen,
            Magenta,
            Maroon,
            MediumAquamarine,
            MediumBlue,
            MediumOrchid,
            MediumPurple,
            MediumSeaGreen,
            MediumSlateBlue,
            MediumSpringGreen,
            MediumTurquoise,
            MediumVioletRed,
            MidnightBlue,
            MintCream,
            MistyRose,
            Moccasin,
            NavajoWhite,
            Navy,
            OldLace,
            Olive,
            OliveDrab,
            Orange,
            OrangeRed,
            Orchid,
            PaleGoldenrod,
            PaleGreen,
            PaleTurquoise,
            PaleVioletRed,
            PapayaWhip,
            PeachPuff,
            Peru,
            Pink,
            Plum,
            PowderBlue,
            Purple,
            Red,
            RosyBrown,
            RoyalBlue,
            SaddleBrown,
            Salmon,
            SandyBrown,
            SeaGreen,
            SeaShell,
            Sienna,
            Silver,
            SkyBlue,
            SlateBlue,
            SlateGray,
            Snow,
            SpringGreen,
            SteelBlue,
            Tan,
            Teal,
            Thistle,
            Tomato,
            Transparent,
            Turquoise,
            Violet,
            Wheat,
            White,
            WhiteSmoke,
            Yellow,
            YellowGreen

        }

        public enum CapitalTo
        {

            Balance,
            Equity

        }

        #endregion

        #region Class

        public class Monitor
        {

            private readonly Positions _allPositions = null;

            public class Information
            {

                public double TotalNetProfit = 0;
                public double MinVolumeInUnits = 0;
                public double MaxVolumeInUnits = 0;
                public double MidVolumeInUnits = 0;
                public int BuyPositions = 0;
                public int SellPositions = 0;
                public Position FirstPosition = null;
                public Position LastPosition = null;

            }


            public class PauseTimes
            {

                public double Over = 0;
                public double Under = 0;

            }

            public class BreakEvenData
            {

                public double Activation = 0;
                public double Distance = 0;

            }

            public class TrailingData
            {

                public double Activation = 0;
                public double Distance = 0;

            }

            public bool OpenedInThisBar = false;

            public readonly string Label;

            public readonly Symbol Symbol;

            public readonly Bars Bars;

            public readonly PauseTimes Pause;

            public Information Info { get; private set; }

            public Position[] Positions { get; private set; }

            public Monitor(string NewLabel, Symbol NewSymbol, Bars NewBars, Positions AllPositions, PauseTimes NewPause)
            {

                Label = NewLabel;
                Symbol = NewSymbol;
                Bars = NewBars;
                Pause = NewPause;

                _allPositions = AllPositions;

                Update(false, null, null);

            }

            public Information Update(bool closeall, BreakEvenData breakevendata, TrailingData trailingdata, TradeType? filtertype = null)
            {

                Positions = _allPositions.FindAll(Label, Symbol.Name);

                Info = new Information();

                double tmpVolume = 0;

                foreach (Position position in Positions)
                {

                    if (closeall && (filtertype == null || position.TradeType == filtertype))
                    {

                        position.Close();
                        continue;

                    }

                    CheckBreakEven(position, breakevendata);

                    CheckTrailing(position, trailingdata);

                    Info.TotalNetProfit += position.NetProfit;
                    tmpVolume += position.VolumeInUnits;

                    switch (position.TradeType)
                    {
                        case TradeType.Buy:

                            Info.BuyPositions++;
                            break;

                        case TradeType.Sell:

                            Info.SellPositions++;
                            break;

                    }

                    if (Info.FirstPosition == null || position.EntryTime < Info.FirstPosition.EntryTime)
                        Info.FirstPosition = position;

                    if (Info.LastPosition == null || position.EntryTime > Info.LastPosition.EntryTime)
                        Info.LastPosition = position;

                    if (Info.MinVolumeInUnits == 0 || position.VolumeInUnits < Info.MinVolumeInUnits)
                        Info.MinVolumeInUnits = position.VolumeInUnits;

                    if (Info.MaxVolumeInUnits == 0 || position.VolumeInUnits > Info.MaxVolumeInUnits)
                        Info.MaxVolumeInUnits = position.VolumeInUnits;

                }

                // --> Restituisce una Exception Overflow di una operazione aritmetica, da approfondire
                //     Info.MidVolumeInUnits = Symbol.NormalizeVolumeInUnits(tmpVolume / Positions.Length,RoundingMode.ToNearest);
                Info.MidVolumeInUnits = Math.Round(tmpVolume / Positions.Length, 0);

                return Info;

            }

            public void CloseAllPositions(TradeType? filtertype = null)
            {

                Update(true, null, null, filtertype);

            }

            public bool InGAP(double distance)
            {

                return Symbol.DigitsToPips(Bars.LastGAP()) >= distance;

            }

            public bool InPause(DateTime timeserver)
            {

                string nowHour = (timeserver.Hour < 10) ? string.Format("0{0}", timeserver.Hour) : string.Format("{0}", timeserver.Hour);
                string nowMinute = (timeserver.Minute < 10) ? string.Format("0{0}", timeserver.Minute) : string.Format("{0}", timeserver.Minute);

                double adesso = Convert.ToDouble(string.Format("{0},{1}", nowHour, nowMinute));

                if (Pause.Over < Pause.Under && adesso >= Pause.Over && adesso <= Pause.Under)
                {

                    return true;

                }
                else if (Pause.Over > Pause.Under && ((adesso >= Pause.Over && adesso <= 23.59) || adesso <= Pause.Under))
                {

                    return true;

                }

                return false;

            }

            private void CheckBreakEven(Position position, BreakEvenData breakevendata)
            {

                if (breakevendata == null || breakevendata.Activation == 0)
                    return;

                switch (position.TradeType)
                {

                    case TradeType.Buy:

                        if ((Symbol.Bid >= (position.EntryPrice + Symbol.PipsToDigits(breakevendata.Activation))) && (position.StopLoss == null || position.StopLoss < position.EntryPrice))
                        {

                            if (breakevendata.Distance == 0)
                            {

                                position.ModifyStopLossPrice(position.EntryPrice);

                            }
                            else
                            {

                                position.ModifyStopLossPips(breakevendata.Distance * -1);

                            }

                        }

                        break;

                    case TradeType.Sell:

                        if ((Symbol.Ask <= (position.EntryPrice - Symbol.PipsToDigits(breakevendata.Activation))) && (position.StopLoss == null || position.StopLoss > position.EntryPrice))
                        {

                            if (breakevendata.Distance == 0)
                            {

                                position.ModifyStopLossPrice(position.EntryPrice);

                            }
                            else
                            {

                                position.ModifyStopLossPips(breakevendata.Distance * -1);

                            }

                        }

                        break;

                }

            }

            private void CheckTrailing(Position position, TrailingData trailingdata)
            {

                if (trailingdata == null || trailingdata.Activation == 0 || trailingdata.Distance == 0)
                    return;

                double trailing;

                switch (position.TradeType)
                {

                    case TradeType.Buy:

                        trailing = Math.Round(Symbol.Bid - Symbol.PipsToDigits(trailingdata.Distance), Symbol.Digits);

                        if ((Symbol.Bid >= (position.EntryPrice + Symbol.PipsToDigits(trailingdata.Activation))) && (position.StopLoss == null || position.StopLoss < trailing))
                        {

                            position.ModifyStopLossPrice(trailing);

                        }

                        break;

                    case TradeType.Sell:

                        trailing = Math.Round(Symbol.Ask + Symbol.PipsToDigits(trailingdata.Distance), Symbol.Digits);

                        if ((Symbol.Ask <= (position.EntryPrice - Symbol.PipsToDigits(trailingdata.Activation))) && (position.StopLoss == null || position.StopLoss > trailing))
                        {

                            position.ModifyStopLossPrice(trailing);

                        }

                        break;

                }

            }

        }

        public class MonenyManagement
        {

            private readonly double _minSize = 0.01;
            private double _percentage = 0;
            private double _fixedSize = 0;
            private double _pipToCalc = 30;

            private readonly IAccount _account = null;
            public readonly Symbol Symbol;

            public CapitalTo CapitalType = CapitalTo.Balance;

            public double Percentage
            {

                get { return _percentage; }


                set { _percentage = (value > 0 && value <= 100) ? value : 0; }
            }

            public double FixedSize
            {

                get { return _fixedSize; }



                set { _fixedSize = (value >= _minSize) ? value : 0; }
            }

            public double PipToCalc
            {

                get { return _pipToCalc; }

                set { _pipToCalc = (value > 0) ? value : 100; }
            }

            public double Capital
            {

                get
                {

                    switch (CapitalType)
                    {

                        case CapitalTo.Equity:

                            return _account.Equity;
                        default:


                            return _account.Balance;

                    }

                }
            }

            public MonenyManagement(IAccount NewAccount, CapitalTo NewCapitalTo, double NewPercentage, double NewFixedSize, double NewPipToCalc, Symbol NewSymbol)
            {

                _account = NewAccount;

                Symbol = NewSymbol;

                CapitalType = NewCapitalTo;
                Percentage = NewPercentage;
                FixedSize = NewFixedSize;
                PipToCalc = NewPipToCalc;

            }

            public double GetLotSize()
            {

                if (FixedSize > 0)
                    return FixedSize;

                double moneyrisk = Capital / 100 * Percentage;

                double sl_double = PipToCalc * Symbol.PipSize;

                // --> In formato 0.01 = microlotto double lots = Math.Round(Symbol.VolumeInUnitsToQuantity(moneyrisk / ((sl_double * Symbol.TickValue) / Symbol.TickSize)), 2);
                // --> In formato volume 1K = 1000 Math.Round((moneyrisk / ((sl_double * Symbol.TickValue) / Symbol.TickSize)), 2);
                double lots = Math.Round(Symbol.VolumeInUnitsToQuantity(moneyrisk / ((sl_double * Symbol.TickValue) / Symbol.TickSize)), 2);

                if (lots < _minSize)
                    return _minSize;

                return lots;

            }

        }

        #endregion

        #region Helper

        public static API.Color ColorFromEnum(ColorNameEnum colorName)
        {

            return API.Color.FromName(colorName.ToString("G"));

        }

        #endregion

        #region Bars

        public static int GetIndexByDate(this Bars thisBars, DateTime thisTime)
        {

            for (int i = thisBars.ClosePrices.Count - 1; i >= 0; i--)
            {

                if (thisTime == thisBars.OpenTimes[i])
                    return i;

            }

            return -1;

        }

        public static double LastGAP(this Bars thisBars)
        {

            double K = 0;

            if (thisBars.ClosePrices.Last(1) > thisBars.OpenPrices.LastValue)
            {

                K = Math.Round(thisBars.ClosePrices.Last(1) - thisBars.OpenPrices.LastValue);

            }
            else if (thisBars.ClosePrices.Last(1) < thisBars.OpenPrices.LastValue)
            {

                K = Math.Round(thisBars.OpenPrices.LastValue - thisBars.ClosePrices.Last(1));

            }

            return K;

        }

        #endregion

        #region Bar

        public static double Body(this Bar thisBar)
        {

            return thisBar.IsBullish() ? thisBar.Close - thisBar.Open : thisBar.Open - thisBar.Close;


        }

        public static bool IsBullish(this Bar thisBar)
        {

            return thisBar.Close > thisBar.Open;

        }

        public static bool IsBearish(this Bar thisBar)
        {

            return thisBar.Close < thisBar.Open;

        }

        public static bool IsDoji(this Bar thisBar)
        {

            return thisBar.Close == thisBar.Open;

        }

        #endregion

        #region Symbol

        public static double DigitsToPips(this Symbol thisSymbol, double Pips)
        {

            return Math.Round(Pips / thisSymbol.PipSize, 2);

        }

        public static double PipsToDigits(this Symbol thisSymbol, double Pips)
        {

            return Math.Round(Pips * thisSymbol.PipSize, thisSymbol.Digits);

        }

        public static double RealSpread(this Symbol thisSymbol)
        {

            return Math.Round(thisSymbol.Spread / thisSymbol.PipSize, 2);

        }

        #endregion

        #region Chart

        public static bool CanDraw(RunningMode thisRunning)
        {

            return thisRunning == RunningMode.RealTime || thisRunning == RunningMode.VisualBacktesting;

        }

        #endregion

        #region TimeFrame

        public static int ToMinutes(this TimeFrame thisTimeFrame)
        {

            if (thisTimeFrame == TimeFrame.Daily)
                return 60 * 24;
            if (thisTimeFrame == TimeFrame.Day2)
                return 60 * 24 * 2;
            if (thisTimeFrame == TimeFrame.Day3)
                return 60 * 24 * 3;
            if (thisTimeFrame == TimeFrame.Hour)
                return 60;
            if (thisTimeFrame == TimeFrame.Hour12)
                return 60 * 12;
            if (thisTimeFrame == TimeFrame.Hour2)
                return 60 * 2;
            if (thisTimeFrame == TimeFrame.Hour3)
                return 60 * 3;
            if (thisTimeFrame == TimeFrame.Hour4)
                return 60 * 4;
            if (thisTimeFrame == TimeFrame.Hour6)
                return 60 * 6;
            if (thisTimeFrame == TimeFrame.Hour8)
                return 60 * 8;
            if (thisTimeFrame == TimeFrame.Minute)
                return 1;
            if (thisTimeFrame == TimeFrame.Minute10)
                return 10;
            if (thisTimeFrame == TimeFrame.Minute15)
                return 15;
            if (thisTimeFrame == TimeFrame.Minute2)
                return 2;
            if (thisTimeFrame == TimeFrame.Minute20)
                return 20;
            if (thisTimeFrame == TimeFrame.Minute3)
                return 3;
            if (thisTimeFrame == TimeFrame.Minute30)
                return 30;
            if (thisTimeFrame == TimeFrame.Minute4)
                return 4;
            if (thisTimeFrame == TimeFrame.Minute45)
                return 45;
            if (thisTimeFrame == TimeFrame.Minute5)
                return 5;
            if (thisTimeFrame == TimeFrame.Minute6)
                return 6;
            if (thisTimeFrame == TimeFrame.Minute7)
                return 7;
            if (thisTimeFrame == TimeFrame.Minute8)
                return 8;
            if (thisTimeFrame == TimeFrame.Minute9)
                return 9;
            if (thisTimeFrame == TimeFrame.Monthly)
                return 60 * 24 * 30;
            if (thisTimeFrame == TimeFrame.Weekly)
                return 60 * 24 * 7;

            return 0;

        }

        #endregion

    }

}

namespace cAlgo.Robots
{

    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class Engulfing : Robot
    {

        #region Enums

        public enum MyTradeType
        {

            Disabled,
            Buy,
            Sell

        }

        #endregion

        #region Identity

        public const string NAME = "Engulfing";

        public const string VERSION = "1.0.4";

        #endregion

        #region Params

        [Parameter(NAME + " " + VERSION, Group = "Identity", DefaultValue = "https://www.google.com/search?q=ctrader+guru+engulfing")]
        public string ProductInfo { get; set; }

        [Parameter("Label ( Magic Name )", Group = "Identity", DefaultValue = NAME)]
        public string MyLabel { get; set; }

        [Parameter("Stop Loss (pips)", Group = "Strategy", DefaultValue = 100, MinValue = 0, Step = 0.1)]
        public double SL { get; set; }

        [Parameter("Take Profit (pips)", Group = "Strategy", DefaultValue = 100, MinValue = 0, Step = 0.1)]
        public double TP { get; set; }

        [Parameter("Money Target (zero disabled)", Group = "Strategy", DefaultValue = 0, MinValue = 0, Step = 0.1)]
        public double MoneyTarget { get; set; }

        [Parameter("Slippage (pips)", Group = "Strategy", DefaultValue = 2.0, MinValue = 0.5, Step = 0.1)]
        public double SLIPPAGE { get; set; }

        [Parameter("Activation (pips)", Group = "Break Even", DefaultValue = 30, MinValue = 0, Step = 0.1)]
        public double BreakEvenActivation { get; set; }

        [Parameter("Distance (pips, move Stop Loss)", Group = "Break Even", DefaultValue = 1.5, Step = 0.1)]
        public double BreakEvenDistance { get; set; }

        [Parameter("Activation (pips)", Group = "Trailing", DefaultValue = 40, MinValue = 0, Step = 0.1)]
        public double TrailingActivation { get; set; }

        [Parameter("Distance (pips, move Stop Loss)", Group = "Trailing", DefaultValue = 30, MinValue = 1, Step = 0.1)]
        public double TrailingDistance { get; set; }

        [Parameter("Fixed Lots", Group = "Money Management", DefaultValue = 0, MinValue = 0, Step = 0.01)]
        public double FixedLots { get; set; }

        [Parameter("Capital", Group = "Money Management", DefaultValue = Extensions.CapitalTo.Balance)]
        public Extensions.CapitalTo MyCapital { get; set; }

        [Parameter("% Risk", Group = "Money Management", DefaultValue = 1, MinValue = 0.1, Step = 0.1)]
        public double MyRisk { get; set; }

        [Parameter("Pips To Calculate ( if no stoploss, empty = '100' )", Group = "Money Management", DefaultValue = 100, MinValue = 0, Step = 0.1)]
        public double FakeSL { get; set; }

        [Parameter("Max Spread allowed", Group = "Filters", DefaultValue = 1.5, MinValue = 0.1, Step = 0.1)]
        public double SpreadToTrigger { get; set; }

        [Parameter("Pause over this time", Group = "Filters", DefaultValue = 21.3, MinValue = 0, MaxValue = 23.59)]
        public double PauseOver { get; set; }

        [Parameter("Pause under this time", Group = "Filters", DefaultValue = 3, MinValue = 0, MaxValue = 23.59)]
        public double PauseUnder { get; set; }

        [Parameter("Max GAP Allowed (pips)", Group = "Filters", DefaultValue = 1, MinValue = 0, Step = 0.01)]
        public double GAP { get; set; }

        [Parameter("Max Number of Trades", Group = "Filters", DefaultValue = 1, MinValue = 1, Step = 1)]
        public int MaxTrades { get; set; }

        [Parameter("Minimum Husband Body", Group = "Filters", DefaultValue = 6.9, MinValue = 0.1)]
        public double MinHusband { get; set; }

        [Parameter("Maximum Husband Body", Group = "Filters", DefaultValue = 23, MinValue = 0.1)]
        public double MaxHusband { get; set; }

        [Parameter("Minimum Wife Body %", Group = "Filters", DefaultValue = 11, MinValue = 0.1)]
        public double WifePercent { get; set; }

        [Parameter("Maximum Wife Body %", Group = "Filters", DefaultValue = 77, MinValue = 0.1)]
        public double WifePercentMax { get; set; }

        [Parameter("EMA Fast Period", Group = "Filters", DefaultValue = 200, MinValue = 2)]
        public int EmaFastPeriod { get; set; }

        [Parameter("EMA Slow Period", Group = "Filters", DefaultValue = 500, MinValue = 10)]
        public int EmaSlowPeriod { get; set; }

        [Parameter("TOP Period", Group = "Filters", DefaultValue = 7, MinValue = 0)]
        public int TOP { get; set; }

        [Parameter("Open Position On Start", Group = "Debug", DefaultValue = MyTradeType.Disabled)]
        public MyTradeType OpenOnStart { get; set; }

        [Parameter("Color Text", Group = "Styles", DefaultValue = Extensions.ColorNameEnum.Coral)]
        public Extensions.ColorNameEnum TextColor { get; set; }

        #endregion

        #region Property

        Extensions.Monitor.PauseTimes Pause1;
        Extensions.Monitor Monitor1;
        Extensions.MonenyManagement MonenyManagement1;
        Extensions.Monitor.BreakEvenData BreakEvenData1;
        Extensions.Monitor.TrailingData TrailingData1;

        ExponentialMovingAverage EMAfast;
        ExponentialMovingAverage EMAslow;
        ParabolicSAR SAR;

        #endregion

        #region cBot Events

        protected override void OnStart()
        {

            Print("{0} : {1}", NAME, VERSION);

            Pause1 = new Extensions.Monitor.PauseTimes 
            {

                Over = PauseOver,
                Under = PauseUnder

            };

            Monitor1 = new Extensions.Monitor(MyLabel, Symbol, Bars, Positions, Pause1);

            MonenyManagement1 = new Extensions.MonenyManagement(Account, MyCapital, MyRisk, FixedLots, SL > 0 ? SL : FakeSL, Symbol);

            BreakEvenData1 = new Extensions.Monitor.BreakEvenData 
            {

                Activation = BreakEvenActivation,
                Distance = BreakEvenDistance

            };

            TrailingData1 = new Extensions.Monitor.TrailingData 
            {

                Activation = TrailingActivation,
                Distance = TrailingDistance

            };

            Positions.Opened += OnOpenPositions;

            EMAfast = Indicators.ExponentialMovingAverage(Bars.ClosePrices, EmaFastPeriod);
            EMAslow = Indicators.ExponentialMovingAverage(Bars.ClosePrices, EmaSlowPeriod);
            SAR = Indicators.ParabolicSAR(0.02, 0.2);

            if (OpenOnStart != MyTradeType.Disabled)
                Test((OpenOnStart == MyTradeType.Buy) ? TradeType.Buy : TradeType.Sell, MonenyManagement1);

        }

        protected override void OnStop()
        {

            Positions.Opened -= OnOpenPositions;

        }

        protected override void OnBar()
        {

            Monitor1.OpenedInThisBar = false;

            Loop(Monitor1, MonenyManagement1);

        }

        protected override void OnTick()
        {


            Monitor1.Update(CheckClosePositions(Monitor1), BreakEvenData1, TrailingData1, null);

        }

        #endregion

        #region Private Methods

        private void OnOpenPositions(PositionOpenedEventArgs eventArgs)
        {

            if (eventArgs.Position.SymbolName == Monitor1.Symbol.Name && eventArgs.Position.Label == Monitor1.Label)
            {

                Monitor1.OpenedInThisBar = true;

            }

        }

        private void Loop(Extensions.Monitor monitor, Extensions.MonenyManagement moneymanagement)
        {

            bool sharedCondition = (!monitor.OpenedInThisBar && !monitor.InGAP(GAP) && !monitor.InPause(Server.Time) && monitor.Symbol.RealSpread() <= SpreadToTrigger && monitor.Positions.Length < MaxTrades);

            bool triggerBuy = CalculateLongTrigger(CalculateLongFilter(sharedCondition));
            bool triggerSell = CalculateShortTrigger(CalculateShortFilter(sharedCondition));

            if (triggerBuy && triggerSell)
            {

                Print("{0} {1} ERROR : trigger buy and sell !", monitor.Label, monitor.Symbol.Name);
                return;

            }

            double volumeInUnits = Monitor1.Symbol.QuantityToVolumeInUnits(moneymanagement.GetLotSize());

            if (triggerBuy)
            {

                ExecuteMarketRangeOrder(TradeType.Buy, monitor.Symbol.Name, volumeInUnits, SLIPPAGE, monitor.Symbol.Ask, monitor.Label, SL, TP);

            }
            else if (triggerSell)
            {

                ExecuteMarketRangeOrder(TradeType.Sell, monitor.Symbol.Name, volumeInUnits, SLIPPAGE, monitor.Symbol.Bid, monitor.Label, SL, TP);

            }

        }

        #endregion

        #region Strategy

        private bool CheckClosePositions(Extensions.Monitor monitor)
        {

            return (MoneyTarget > 0 && monitor.Info.TotalNetProfit >= MoneyTarget);

        }

        private bool CalculateLongFilter(bool condition = true)
        {

            if (!condition)
                return false;

            return (SAR.Result.Last(2) > Bars.HighPrices.Last(2) && SAR.Result.Last(1) > Bars.HighPrices.Last(1));

        }

        private bool CalculateShortFilter(bool condition = true)
        {

            if (!condition)
                return false;

            return (SAR.Result.Last(2) < Bars.LowPrices.Last(2) && SAR.Result.Last(1) < Bars.LowPrices.Last(1));

        }

        private bool CalculateLongTrigger(bool filter = true)
        {

            if (!filter)
                return false;

            return HaveEngulfingBullish();

        }

        private bool CalculateShortTrigger(bool filter = true)
        {

            if (!filter)
                return false;

            return HaveEngulfingBearish();

        }

        private void Test(TradeType trigger, Extensions.MonenyManagement moneymanagement)
        {

            double volumeInUnits = moneymanagement.Symbol.QuantityToVolumeInUnits(moneymanagement.GetLotSize());

            switch (trigger)
            {

                case TradeType.Buy:

                    ExecuteMarketRangeOrder(TradeType.Buy, moneymanagement.Symbol.Name, volumeInUnits, SLIPPAGE, moneymanagement.Symbol.Ask, "TEST", SL, TP);
                    break;

                case TradeType.Sell:

                    ExecuteMarketRangeOrder(TradeType.Sell, moneymanagement.Symbol.Name, volumeInUnits, SLIPPAGE, moneymanagement.Symbol.Bid, "TEST", SL, TP);
                    break;

            }

        }

        private double GetPercentageOf(double fromP, double toP)
        {

            return Math.Round(fromP / (toP / 100), 2);

        }

        private bool HaveEngulfingBullish()
        {

            double wifeBody = (Bars.OpenPrices.Last(2) - Bars.ClosePrices.Last(2)) / Symbol.PipSize;
            double husbandBody = (Bars.ClosePrices.Last(1) - Bars.OpenPrices.Last(1)) / Symbol.PipSize;

            if (Bars.HighPrices.Last(2) >= Bars.HighPrices.Last(1) || Bars.LowPrices.Last(2) < Bars.LowPrices.Last(1))
                return false;

            double tmp = GetPercentageOf(wifeBody, husbandBody);

            if (wifeBody <= 0 || husbandBody <= 0 || husbandBody < MinHusband || husbandBody > MaxHusband || wifeBody >= husbandBody || tmp < WifePercent || tmp > WifePercentMax)
                return false;
            if (Bars.HighPrices.Last(1) >= EMAfast.Result.Last(1))
                return false;
            if (!(Symbol.Ask < EMAfast.Result.LastValue && Symbol.Ask < EMAslow.Result.LastValue && EMAfast.Result.LastValue < EMAslow.Result.LastValue))
                return false;
            if (TOP > 0 && Bars.LowPrices.Minimum(TOP) != Bars.LowPrices.Last(1))
                return false;

            return true;

        }

        private bool HaveEngulfingBearish()
        {

            double wifeBody = (Bars.ClosePrices.Last(2) - Bars.OpenPrices.Last(2)) / Symbol.PipSize;
            double husbandBody = (Bars.OpenPrices.Last(1) - Bars.ClosePrices.Last(1)) / Symbol.PipSize;

            if (Bars.HighPrices.Last(2) >= Bars.HighPrices.Last(1) || Bars.LowPrices.Last(2) < Bars.LowPrices.Last(1))
                return false;

            double tmp = GetPercentageOf(wifeBody, husbandBody);

            if (wifeBody <= 0 || husbandBody <= 0 || husbandBody < MinHusband || husbandBody > MaxHusband || wifeBody >= husbandBody || tmp < WifePercent || tmp > WifePercentMax)
                return false;
            if (!(Symbol.Bid > EMAfast.Result.LastValue && Symbol.Bid > EMAslow.Result.LastValue && EMAfast.Result.LastValue > EMAslow.Result.LastValue))
                return false;
            if (Bars.LowPrices.Last(1) <= EMAfast.Result.Last(1))
                return false;
            if (TOP > 0 && Bars.HighPrices.Maximum(TOP) != Bars.HighPrices.Last(1))
                return false;

            return true;

        }


        #endregion

    }

}
