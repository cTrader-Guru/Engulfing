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
    /// <summary>
    /// Estensioni che rendono il codice più scorrevole con metodi non previsti dalla libreria cAlgo
    /// </summary>
    public static class Extensions
    {

        #region Enum

        /// <summary>
        /// Enumeratore per esporre il nome del colore nelle opzioni
        /// </summary>
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

        /// <summary>
        /// Enumeratore per esporre nei parametri una scelta con menu a tendina
        /// </summary>
        public enum CapitalTo
        {

            Balance,
            Equity

        }

        #endregion

        #region Class

        /// <summary>
        /// Classe per monitorare le posizioni di una specifica strategia
        /// </summary>
        public class Monitor
        {

            private Positions _allPositions = null;

            /// <summary>
            /// Standard per la raccolta di informazioni nel Monitor
            /// </summary>
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

            /// <summary>
            /// Standard per l'interpretazione dell'orario in double
            /// </summary>
            public class PauseTimes
            {

                public double Over = 0;
                public double Under = 0;

            }

            /// <summary>
            /// Standard per la gestione del break even
            /// </summary>
            public class BreakEvenData
            {

                public double Activation = 0;
                public double Distance = 0;

            }

            /// <summary>
            /// Standard per la gestione del trailing
            /// </summary>
            public class TrailingData
            {

                public double Activation = 0;
                public double Distance = 0;

            }

            /// <summary>
            /// Memorizza lo stato di apertura di una operazione nella Bar corrente
            /// </summary>
            public bool OpenedInThisBar = false;

            /// <summary>
            /// Valore univoco che identifica la strategia
            /// </summary>
            public readonly string Label;

            /// <summary>
            /// Il Simbolo da monitorare in relazione alla Label
            /// </summary>
            public readonly Symbol Symbol;

            /// <summary>
            /// Le Bars con il quale la strategia si muove ed elabora le sue condizioni
            /// </summary>
            public readonly Bars Bars;

            /// <summary>
            /// Il riferimento temporale della pausa
            /// </summary>
            public readonly PauseTimes Pause;

            /// <summary>
            /// Le informazioni raccolte dopo la chiamata .Update()
            /// </summary>
            public Information Info { get; private set; }

            /// <summary>
            /// Le posizioni filtrate in base al simbolo e alla label
            /// </summary>
            public Position[] Positions { get; private set; }

            /// <summary>
            /// Monitor per la raccolta d'informazioni inerenti la strategia in corso
            /// </summary>
            public Monitor(string NewLabel, Symbol NewSymbol, Bars NewBars, Positions AllPositions, PauseTimes NewPause)
            {

                Label = NewLabel;
                Symbol = NewSymbol;
                Bars = NewBars;
                Pause = NewPause;

                _allPositions = AllPositions;

                // --> Rendiamo sin da subito disponibili le informazioni
                Update(false, null, null);

            }

            /// <summary>
            /// Filtra e rende disponibili le informazioni per la strategia monitorata. Eventualmente Chiude e gestisce le operazioni
            /// </summary>
            public Information Update(bool closeall, BreakEvenData breakevendata, TrailingData trailingdata, TradeType? filtertype = null)
            {

                // --> Raccolgo le informazioni che mi servono per avere il polso della strategia
                Positions = _allPositions.FindAll(Label, Symbol.Name);

                // --> Resetto le informazioni
                Info = new Information();

                double tmpVolume = 0;

                foreach (Position position in Positions)
                {

                    // --> Per prima cosa devo controllare se chiudere la posizione
                    if (closeall && (filtertype == null || position.TradeType == filtertype))
                    {

                        position.Close();
                        continue;

                    }

                    // --> Poi tocca al break even
                    _checkBreakEven(position, breakevendata);

                    // --> Poi tocca al trailing
                    _checkTrailing(position, trailingdata);

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

            /// <summary>
            /// Chiude tutte le posizioni del monitor
            /// </summary>
            public void CloseAllPositions(TradeType? filtertype = null)
            {

                Update(true, null, null, filtertype);

            }

            /// <summary>
            /// Stabilisce se si è in GAP passando una certa distanza da misurare
            /// </summary>
            public bool InGAP(double distance)
            {

                return Symbol.DigitsToPips(Bars.LastGAP()) >= distance;

            }

            /// <summary>
            /// Controlla la fascia oraria per determinare se rientra in quella di pausa, utilizza dati double 
            /// perchè la ctrader non permette di esporre dati time, da aggiornare non appena la ctrader lo permette
            /// </summary>
            /// <returns>Conferma la presenza di una fascia oraria in pausa</returns>
            public bool InPause(DateTime timeserver)
            {

                // -->> Poichè si utilizzano dati double per esporre i parametri dobbiamo utilizzare meccanismi per tradurre l'orario
                string nowHour = (timeserver.Hour < 10) ? string.Format("0{0}", timeserver.Hour) : string.Format("{0}", timeserver.Hour);
                string nowMinute = (timeserver.Minute < 10) ? string.Format("0{0}", timeserver.Minute) : string.Format("{0}", timeserver.Minute);

                // --> Stabilisco il momento di controllo in formato double
                double adesso = Convert.ToDouble(string.Format("{0},{1}", nowHour, nowMinute));

                // --> Confronto elementare per rendere comprensibile la logica
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

            /// <summary>
            /// Controlla ed effettua la modifica in break-even se le condizioni le permettono
            /// </summary>
            private void _checkBreakEven(Position position, BreakEvenData breakevendata)
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


            /// <summary>
            /// Controlla ed effettua la modifica in trailing se le condizioni le permettono
            /// </summary>
            private void _checkTrailing(Position position, TrailingData trailingdata)
            {

                if (trailingdata == null || trailingdata.Activation == 0 || trailingdata.Distance == 0)
                    return;

                double trailing = 0;

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

        /// <summary>
        /// Classe per gestire il dimensionamento delle size
        /// </summary>
        public class MonenyManagement
        {

            private readonly double _minSize = 0.01;
            private double _percentage = 0;
            private double _fixedSize = 0;
            private double _pipToCalc = 30;

            // --> Riferimenti agli oggetti esterni utili per il calcolo
            private IAccount _account = null;
            public readonly Symbol Symbol;

            /// <summary>
            /// Il capitale da utilizzare per il calcolo
            /// </summary>
            public CapitalTo CapitalType = CapitalTo.Balance;

            /// <summary>
            /// La percentuale di rischio che si vuole investire
            /// </summary>
            public double Percentage
            {

                get { return _percentage; }


                set { _percentage = (value > 0 && value <= 100) ? value : 0; }
            }

            /// <summary>
            /// La size fissa da utilizzare, bypassa tutti i parametri di calcolo
            /// </summary>
            public double FixedSize
            {

                get { return _fixedSize; }



                set { _fixedSize = (value >= _minSize) ? value : 0; }
            }


            /// <summary>
            /// La distanza massima dall'ingresso con il quale calcolare le size
            /// </summary>
            public double PipToCalc
            {

                get { return _pipToCalc; }

                set { _pipToCalc = (value > 0) ? value : 100; }
            }


            /// <summary>
            /// Il capitale effettivo sul quale calcolare il rischio
            /// </summary>
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



            // --> Costruttore
            public MonenyManagement(IAccount NewAccount, CapitalTo NewCapitalTo, double NewPercentage, double NewFixedSize, double NewPipToCalc, Symbol NewSymbol)
            {

                _account = NewAccount;

                Symbol = NewSymbol;

                CapitalType = NewCapitalTo;
                Percentage = NewPercentage;
                FixedSize = NewFixedSize;
                PipToCalc = NewPipToCalc;

            }

            /// <summary>
            /// Restituisce il numero di lotti in formato 0.01
            /// </summary>
            public double GetLotSize()
            {

                // --> Hodeciso di usare una size fissa
                if (FixedSize > 0)
                    return FixedSize;

                // --> La percentuale di rischio in denaro
                double moneyrisk = Capital / 100 * Percentage;

                // --> Traduco lo stoploss o il suo riferimento in double
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

        /// <summary>
        /// Restituisce il colore corrispondente a partire dal nome
        /// </summary>
        /// <returns>Il colore corrispondente</returns>
        public static API.Color ColorFromEnum(ColorNameEnum colorName)
        {

            return API.Color.FromName(colorName.ToString("G"));

        }

        #endregion

        #region Bars

        /// <summary>
        /// Si ottiene l'indice della candela partendo dal suo orario di apertura
        /// </summary>
        /// <param name="MyTime">La data e l'ora di apertura della candela</param>
        /// <returns></returns>
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

        /// <summary>
        /// Misura la grandezza di una candela, tenendo conto della sua direzione
        /// </summary>
        /// <returns>Il corpo della candela, valore uguale o superiore a zero</returns>
        public static double Body(this Bar thisBar)
        {

            return thisBar.IsBullish() ? thisBar.Close - thisBar.Open : thisBar.Open - thisBar.Close;


        }

        /// <summary>
        /// Verifica la direzione rialzista di una candela
        /// </summary>
        /// <returns>True se la candela è rialzista</returns>        
        public static bool IsBullish(this Bar thisBar)
        {

            return thisBar.Close > thisBar.Open;

        }

        /// <summary>
        /// Verifica la direzione ribassista di una candela
        /// </summary>
        /// <returns>True se la candela è ribassista</returns>        
        public static bool IsBearish(this Bar thisBar)
        {

            return thisBar.Close < thisBar.Open;

        }

        /// <summary>
        /// Verifica se una candela ha un open uguale al close
        /// </summary>
        /// <returns>True se la candela è una doji con Open e Close uguali</returns>        
        public static bool IsDoji(this Bar thisBar)
        {

            return thisBar.Close == thisBar.Open;

        }

        #endregion

        #region Symbol

        /// <summary>
        /// Converte il numero di pips corrente da digits a double
        /// </summary>
        /// <param name="Pips">Il numero di pips nel formato Digits</param>
        /// <returns></returns>
        public static double DigitsToPips(this Symbol thisSymbol, double Pips)
        {

            return Math.Round(Pips / thisSymbol.PipSize, 2);

        }

        /// <summary>
        /// Converte il numero di pips corrente da double a digits
        /// </summary>
        /// <param name="Pips">Il numero di pips nel formato Double (2)</param>
        /// <returns></returns>
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

        /// <summary>
        /// Determina se ci sono le condizioni per disegnare sul grafico
        /// </summary>
        public static bool CanDraw(this Chart thisChart, RunningMode thisRunning)
        {

            return thisRunning == RunningMode.RealTime || thisRunning == RunningMode.VisualBacktesting;

        }

        #endregion

        #region TimeFrame

        /// <summary>
        /// Restituisce in minuti il timeframe corrente
        /// </summary>
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

        /// <summary>
        /// Nome del prodotto, identificativo, da modificare con il nome della propria creazione
        /// </summary>
        public const string NAME = "Engulfing";

        /// <summary>
        /// La versione del prodotto, progressivo, utilie per controllare gli aggiornamenti se viene reso disponibile sul sito ctrader.guru
        /// </summary>
        public const string VERSION = "1.0.4";

        #endregion

        #region Params

        /// <summary>
        /// Riferimenti del prodotto
        /// </summary>
        [Parameter(NAME + " " + VERSION, Group = "Identity", DefaultValue = "https://ctrader.guru/product/cbot-base/")]
        public string ProductInfo { get; set; }

        /// <summary>
        /// Label che contraddistingue una operazione
        /// </summary>
        [Parameter("Label ( Magic Name )", Group = "Identity", DefaultValue = NAME)]
        public string MyLabel { get; set; }

        /// <summary>
        /// Lo Stop Loss che verrà utilizzato per ogni operazione
        /// </summary>
        [Parameter("Stop Loss (pips)", Group = "Strategy", DefaultValue = 100, MinValue = 0, Step = 0.1)]
        public double SL { get; set; }

        /// <summary>
        /// Il Take Profit che verrà utilizzato per ogni operazione
        /// </summary>
        [Parameter("Take Profit (pips)", Group = "Strategy", DefaultValue = 100, MinValue = 0, Step = 0.1)]
        public double TP { get; set; }

        /// <summary>
        /// Al raggiungimento di questo netprofit chiude tutto
        /// </summary>
        [Parameter("Money Target (zero disabled)", Group = "Strategy", DefaultValue = 0, MinValue = 0, Step = 0.1)]
        public double MoneyTarget { get; set; }

        /// <summary>
        /// Il broker dovrebbe considerare questo valore come massimo slittamento
        /// </summary>
        [Parameter("Slippage (pips)", Group = "Strategy", DefaultValue = 2.0, MinValue = 0.5, Step = 0.1)]
        public double SLIPPAGE { get; set; }

        /// <summary>
        /// L'attivazione per il moniotraggio del Break Even, se pari a zero disabilita il controllo
        /// </summary>
        [Parameter("Activation (pips)", Group = "Break Even", DefaultValue = 30, MinValue = 0, Step = 0.1)]
        public double BreakEvenActivation { get; set; }

        /// <summary>
        /// Il numero di pips da spostare in caso di attivazione del Break Even, può essere inferiore a zero
        /// </summary>
        [Parameter("Distance (pips, move Stop Loss)", Group = "Break Even", DefaultValue = 1.5, Step = 0.1)]
        public double BreakEvenDistance { get; set; }

        /// <summary>
        /// L'attivazione per il moniotraggio del Trailing, se pari a zero disabilita il controllo
        /// </summary>
        [Parameter("Activation (pips)", Group = "Trailing", DefaultValue = 40, MinValue = 0, Step = 0.1)]
        public double TrailingActivation { get; set; }

        /// <summary>
        /// Il numero di pips che segna la distanza del Trailing, se pari a zero inibisce il Trailing
        /// </summary>
        [Parameter("Distance (pips, move Stop Loss)", Group = "Trailing", DefaultValue = 30, MinValue = 1, Step = 0.1)]
        public double TrailingDistance { get; set; }

        /// <summary>
        /// Valore esclusivo che bypassa il calcolo del rischio, se pari a zero non prende in considerazione il valore manuale
        /// </summary>
        [Parameter("Fixed Lots", Group = "Money Management", DefaultValue = 0, MinValue = 0, Step = 0.01)]
        public double FixedLots { get; set; }

        /// <summary>
        /// Il capitale da prendere in considerazione per il calcolo del rischio
        /// </summary>
        [Parameter("Capital", Group = "Money Management", DefaultValue = Extensions.CapitalTo.Balance)]
        public Extensions.CapitalTo MyCapital { get; set; }

        /// <summary>
        /// La percentuale di rischio da calcolare per la size in lotti
        /// </summary>
        [Parameter("% Risk", Group = "Money Management", DefaultValue = 1, MinValue = 0.1, Step = 0.1)]
        public double MyRisk { get; set; }

        /// <summary>
        /// Il numero di pips da prendere in considerazione se lo Stop Loss è pari a zero per calcolare la size, se
        /// anche questo valore sarà zero allora verrà impostato 100 come valore nominale
        /// </summary>
        [Parameter("Pips To Calculate ( if no stoploss, empty = '100' )", Group = "Money Management", DefaultValue = 100, MinValue = 0, Step = 0.1)]
        public double FakeSL { get; set; }

        /// <summary>
        /// Massimo spread consentito per le operazioni
        /// </summary>
        [Parameter("Max Spread allowed", Group = "Filters", DefaultValue = 1.5, MinValue = 0.1, Step = 0.1)]
        public double SpreadToTrigger { get; set; }

        /// <summary>
        /// Livello temporale espresso in double oltre il quale il cbot entra in pausa
        /// </summary>
        [Parameter("Pause over this time", Group = "Filters", DefaultValue = 21.3, MinValue = 0, MaxValue = 23.59)]
        public double PauseOver { get; set; }

        /// <summary>
        /// Livello temporale espresso in double al di sotto il cbot entra in pausa
        /// </summary>
        [Parameter("Pause under this time", Group = "Filters", DefaultValue = 3, MinValue = 0, MaxValue = 23.59)]
        public double PauseUnder { get; set; }

        /// <summary>
        /// La distanza massima (GAP) in pips che può intercorrere tra una chiusura e una apertura (cambio candela)
        /// </summary>
        [Parameter("Max GAP Allowed (pips)", Group = "Filters", DefaultValue = 1, MinValue = 0, Step = 0.01)]
        public double GAP { get; set; }

        /// <summary>
        /// Il numero massimo di trades che il cbot deve aprire
        /// </summary>
        [Parameter("Max Number of Trades", Group = "Filters", DefaultValue = 1, MinValue = 1, Step = 1)]
        public int MaxTrades { get; set; }

        /// <summary>
        /// La lunghezza minima in pips per il body della candela più grande (Marito)
        /// </summary>
        [Parameter("Minimum Husband Body", Group = "Filters", DefaultValue = 6.9, MinValue = 0.1)]
        public double MinHusband { get; set; }

        /// <summary>
        /// La lunghezza massima in pips per il body della candela più grande (Marito)
        /// </summary>
        [Parameter("Maximum Husband Body", Group = "Filters", DefaultValue = 23, MinValue = 0.1)]
        public double MaxHusband { get; set; }

        /// <summary>
        /// La lunghezza minima in percentuale sul Marito per il body della candela più piccola (Moglie)
        /// </summary>
        [Parameter("Minimum Wife Body %", Group = "Filters", DefaultValue = 11, MinValue = 0.1)]
        public double WifePercent { get; set; }

        /// <summary>
        /// La lunghezza massima in percentuale sul Marito per il body della candela più piccola (Moglie)
        /// </summary>
        [Parameter("Maximum Wife Body %", Group = "Filters", DefaultValue = 77, MinValue = 0.1)]
        public double WifePercentMax { get; set; }

        /// <summary>
        /// Periodi della EMA veloce
        /// </summary>
        [Parameter("EMA Fast Period", Group = "Filters", DefaultValue = 200, MinValue = 2)]
        public int EmaFastPeriod { get; set; }

        /// <summary>
        /// Periodi della EMA lenta
        /// </summary>
        [Parameter("EMA Slow Period", Group = "Filters", DefaultValue = 500, MinValue = 10)]
        public int EmaSlowPeriod { get; set; }

        /// <summary>
        /// Il numero di candele da considerare per ricavare il minimo o il massimo
        /// </summary>
        [Parameter("TOP Period", Group = "Filters", DefaultValue = 7, MinValue = 0)]
        public int TOP { get; set; }

        /// <summary>
        /// Opzione per il debug che apre una posizione di test (label TEST)
        /// </summary>
        [Parameter("Open Position On Start", Group = "Debug", DefaultValue = MyTradeType.Disabled)]
        public MyTradeType OpenOnStart { get; set; }

        /// <summary>
        /// Il colore del testo per eventuali messaggi da stampare sul chart
        /// </summary>
        [Parameter("Color Text", Group = "Styles", DefaultValue = Extensions.ColorNameEnum.Coral)]
        public Extensions.ColorNameEnum TextColor { get; set; }

        #endregion

        #region Property

        Extensions.Monitor.PauseTimes Pause1;
        Extensions.Monitor Monitor1;
        Extensions.MonenyManagement MonenyManagement1;
        Extensions.Monitor.BreakEvenData BreakEvenData1;
        Extensions.Monitor.TrailingData TrailingData1;

        // --> I tre indicatori con cui analizzeremo i filtri e i trigger
        ExponentialMovingAverage EMAfast;
        ExponentialMovingAverage EMAslow;
        ParabolicSAR SAR;

        #endregion

        #region cBot Events

        /// <summary>
        /// Evento generato quando viene avviato il cBot
        /// </summary>
        protected override void OnStart()
        {

            // --> Stampo nei log la versione corrente
            Print("{0} : {1}", NAME, VERSION);

            // --> Messaggio di avvertimento nel caso incui si eseguisse senza modifiche logiche
            /*
            if (Chart.CanDraw(RunningMode))
                Chart.DrawStaticText(NAME, "ATTENTION : CBOT BASE, EDIT THIS TEMPLATE ONLY", VerticalAlignment.Top, HorizontalAlignment.Left, Extensions.ColorFromEnum(TextColor));
            */

            // --> Determino il range di pausa
            Pause1 = new Extensions.Monitor.PauseTimes
            {

                Over = PauseOver,
                Under = PauseUnder

            };

            // --> Inizializzo il Monitor
            Monitor1 = new Extensions.Monitor(MyLabel, Symbol, Bars, Positions, Pause1);

            // --> Inizializzo il MoneyManagement
            MonenyManagement1 = new Extensions.MonenyManagement(Account, MyCapital, MyRisk, FixedLots, SL > 0 ? SL : FakeSL, Symbol);

            // --> Inizializzo i dati per la gestione del breakeven
            BreakEvenData1 = new Extensions.Monitor.BreakEvenData
            {

                Activation = BreakEvenActivation,
                Distance = BreakEvenDistance

            };

            // --> Inizializzo i dati per la gestione del Trailing
            TrailingData1 = new Extensions.Monitor.TrailingData
            {

                Activation = TrailingActivation,
                Distance = TrailingDistance

            };

            // --> Osservo le aperture per operazioni comuni
            Positions.Opened += _onOpenPositions;

            // --> Fissiamo la logica degli indicatori 
            EMAfast = Indicators.ExponentialMovingAverage(Bars.ClosePrices, EmaFastPeriod);
            EMAslow = Indicators.ExponentialMovingAverage(Bars.ClosePrices, EmaSlowPeriod);
            SAR = Indicators.ParabolicSAR(0.02, 0.2);

            // --> Effettuo un test di apertura per verificare il funzionamento del sistema
            if (OpenOnStart != MyTradeType.Disabled) _test((OpenOnStart == MyTradeType.Buy) ? TradeType.Buy : TradeType.Sell, MonenyManagement1);

        }

        /// <summary>
        /// Evento generato quando viene fermato il cBot
        /// </summary>
        protected override void OnStop()
        {

            // --> Meglio eliminare l'handler, non dovrebbe servire ma non si sa mai
            Positions.Opened -= _onOpenPositions;

        }

        /// <summary>
        /// Evento generato ad ogni cambio candela
        /// </summary>
        protected override void OnBar()
        {

            // --> Resetto il flag del controllo candela
            Monitor1.OpenedInThisBar = false;

            _loop(Monitor1, MonenyManagement1, BreakEvenData1, TrailingData1);

        }

        /// <summary>
        /// Evento generato a ogni tick
        /// </summary>
        protected override void OnTick()
        {


            Monitor1.Update(_checkClosePositions(Monitor1), BreakEvenData1, TrailingData1, null);

        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Operazioni da compiere ogni volta che apro una posizione con questa label
        /// </summary>
        private void _onOpenPositions(PositionOpenedEventArgs eventArgs)
        {

            if (eventArgs.Position.SymbolName == Monitor1.Symbol.Name && eventArgs.Position.Label == Monitor1.Label)
            {

                Monitor1.OpenedInThisBar = true;

            }

        }

        private void _loop(Extensions.Monitor monitor, Extensions.MonenyManagement moneymanagement, Extensions.Monitor.BreakEvenData breakevendata, Extensions.Monitor.TrailingData trailingdata)
        {

            // --> Aggiorno le informazioni necessarie per gestire la strategia
            //monitor.Update(_checkClosePositions(monitor), breakevendata, trailingdata, null);


            // --> Condizione condivisa, filtri generali, segnano il perimetro di azione limitando l'ingresso
            bool sharedCondition = (!monitor.OpenedInThisBar && !monitor.InGAP(GAP) && !monitor.InPause(Server.Time) && monitor.Symbol.RealSpread() <= SpreadToTrigger && monitor.Positions.Length < MaxTrades);

            // --> Controllo la presenza di trigger d'ingresso tenendo conto i filtri
            bool triggerBuy = _calculateLongTrigger(_calculateLongFilter(sharedCondition));
            bool triggerSell = _calculateShortTrigger(_calculateShortFilter(sharedCondition));

            // --> Se ho entrambi i trigger qualcosa non va, lo segnalo nei log e fermo la routin
            if (triggerBuy && triggerSell)
            {

                Print("{0} {1} ERROR : trigger buy and sell !", monitor.Label, monitor.Symbol.Name);
                return;

            }

            // --> Calcolo la size in base al money management stabilito
            double volumeInUnits = Monitor1.Symbol.QuantityToVolumeInUnits(moneymanagement.GetLotSize());

            // --> Se ho il segnale d'ingresso considerando i filtri allora procedo con l'ordine a mercato
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

        /// <summary>
        /// Controlla e stabilisce se si devono chiudere tutte le posizioni
        /// </summary>
        private bool _checkClosePositions(Extensions.Monitor monitor)
        {

            // --> Criteri da stabilire con la strategia, monitor.Positions......
            return (MoneyTarget > 0 && monitor.Info.TotalNetProfit >= MoneyTarget);

        }

        /// <summary>
        /// Conferma se i criteri di filtraggio long sono stati soddisfatti
        /// </summary>
        /// <param name="condition">Filtro globale, condizione condivisa</param>
        /// <returns>I filtri long sono stati soddisfatti</returns>
        private bool _calculateLongFilter(bool condition = true)
        {

            // --> La condizione primaria deve essere presente altrimenti non serve continuare
            if (!condition)
                return false;

            // --> Criteri da stabilire
            return (SAR.Result.Last(2) > Bars.HighPrices.Last(2) && SAR.Result.Last(1) > Bars.HighPrices.Last(1));

        }

        /// <summary>
        /// Conferma se i criteri di filtraggio short sono stati soddisfatti
        /// </summary>
        /// <param name="condition">Filtro globale, condizione condivisa</param>
        /// <returns>I filtri short sono stati soddisfatti</returns>
        private bool _calculateShortFilter(bool condition = true)
        {

            // --> La condizione primaria deve essere presente altrimenti non serve continuare
            if (!condition)
                return false;

            // --> Criteri da stabilire
            return (SAR.Result.Last(2) < Bars.LowPrices.Last(2) && SAR.Result.Last(1) < Bars.LowPrices.Last(1));

        }

        /// <summary>
        /// Conferma se i criteri d'ingresso long sono stati soddisfatti
        /// </summary>
        /// <param name="filter">Filtro long, condizione condivisa</param>
        /// <returns>É presente una condizione di apertura long</returns>
        private bool _calculateLongTrigger(bool filter = true)
        {

            // --> Il filtro primario deve essere presente altrimenti non serve continuare
            if (!filter)
                return false;

            // --> Criteri da stabilire
            return _haveEngulfingBullish();

        }

        /// <summary>
        /// Conferma se i criteri d'ingresso short sono stati soddisfatti
        /// </summary>
        /// <param name="filter">Filtro short, condizione condivisa</param>
        /// <returns>É presente una condizione di apertura short</returns>
        private bool _calculateShortTrigger(bool filter = true)
        {

            // --> Il filtro primario deve essere presente altrimenti non serve continuare
            if (!filter)
                return false;

            // --> Criteri da stabilire
            return _haveEngulfingBearish();

        }

        private void _test(TradeType trigger, Extensions.MonenyManagement moneymanagement)
        {

            // --> Calcolo la size in base al money management stabilito
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

        /// <summary>
        /// Restituisce la proporzione in percentuale tra moglie e marito
        /// </summary>
        /// <param name="fromP">Il punto di partenza del calcolo</param>
        /// <param name="toP">Il punto di arrivo del calcolo</param>
        /// <returns>Restituisce la percentuale della proporzione</returns>
        private double _getPercentageOf(double fromP, double toP)
        {

            return Math.Round(fromP / (toP / 100), 2);

        }

        /// <summary>
        /// Confronta le condizioni per la misura del pattern engulfing bullish
        /// </summary>
        /// <returns>Restituisce la condizione soddisfatta o meno</returns>
        private bool _haveEngulfingBullish()
        {

            // --> Per prima cosa le candele devono seguire uno specifico schema, moglie piccola marito grande
            double wifeBody = (Bars.OpenPrices.Last(2) - Bars.ClosePrices.Last(2)) / Symbol.PipSize;
            double husbandBody = (Bars.ClosePrices.Last(1) - Bars.OpenPrices.Last(1)) / Symbol.PipSize;

            if (Bars.HighPrices.Last(2) >= Bars.HighPrices.Last(1) || Bars.LowPrices.Last(2) < Bars.LowPrices.Last(1))
                return false;

            // --> le proporzioni vengono definite dall'utente
            double tmp = _getPercentageOf(wifeBody, husbandBody);

            // --> Debug : Print("{0} - {1} - {2} - {3} - {4} - {5}", wifeBody <= 0, husbandBody <= 0, husbandBody, husbandBody > MaxHusband, wifeBody >= husbandBody, tmp < WifePercent);

            // --> Escludo le condizioni sfavorevoli
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

        /// <summary>
        /// Confronta le condizioni per la misura del pattern engulfing bearish
        /// </summary>
        /// <returns>Restituisce la condizione soddisfatta o meno</returns>
        private bool _haveEngulfingBearish()
        {

            // --> Per prima cosa le candele devono seguire uno specifico schema, moglie piccola marito grande
            double wifeBody = (Bars.ClosePrices.Last(2) - Bars.OpenPrices.Last(2)) / Symbol.PipSize;
            double husbandBody = (Bars.OpenPrices.Last(1) - Bars.ClosePrices.Last(1)) / Symbol.PipSize;

            if (Bars.HighPrices.Last(2) >= Bars.HighPrices.Last(1) || Bars.LowPrices.Last(2) < Bars.LowPrices.Last(1))
                return false;

            // --> le proporzioni vengono definite dall'utente
            double tmp = _getPercentageOf(wifeBody, husbandBody);

            // --> Debug : Print("{0} - {1} - {2} - {3} - {4} - {5}", wifeBody <= 0, husbandBody <= 0, husbandBody, husbandBody > MaxHusband, wifeBody >= husbandBody, tmp < WifePercent);

            // --> Escludo le condizioni sfavorevoli
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
