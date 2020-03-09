/*  CTRADER GURU --> Template 1.0.2

    Homepage    : https://ctrader.guru/
    Telegram    : https://t.me/ctraderguru
    Twitter     : https://twitter.com/cTraderGURU/
    Facebook    : https://www.facebook.com/ctrader.guru/
    YouTube     : https://www.youtube.com/channel/UCKkgbw09Fifj65W5t5lHeCQ
    GitHub      : https://github.com/cTraderGURU/
    TOS         : https://ctrader.guru/termini-del-servizio/

 */

using System;
using cAlgo.API;
using System.Net;
using System.Text;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

// --> Microsoft Visual Studio 2017 --> Strumenti --> Gestione pacchetti NuGet --> Gestisci pacchetti NuGet per la soluzione... --> Installa
using Newtonsoft.Json;
using System.ComponentModel;

namespace cAlgo.Robots
{
    // --> AccessRights = AccessRights.FullAccess se si vuole controllare gli aggiornamenti
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.FullAccess)]
    public class Engulfing : Robot
    {
        #region Enums

        /// <summary>
        /// Selezione per le opzioni della scelta in merito alla base di calcolo
        /// </summary>
        public enum AccCapital
        {
            [Description("Balance")]
            Balance,

            [Description("Free Margin")]
            FreeMargin,

            [Description("Equity")]
            Equity

        }

        #endregion

        #region Identity Params

        /// <summary>
        /// ID prodotto, identificativo, viene fornito da ctrader.guru
        /// </summary>
        public const int ID = 60892;

        /// <summary>
        /// Nome del prodotto, identificativo, da modificare con il nome della propria creazione
        /// </summary>
        public const string NAME = "Engulfing";

        /// <summary>
        /// La versione del prodotto, progressivo, utilie per controllare gli aggiornamenti se viene reso disponibile sul sito ctrader.guru
        /// </summary>
        public const string VERSION = "1.0.2";

        #endregion

        #region Params

        /// <summary>
        /// L'indirizzo del prodotto su ctrader.guru
        /// </summary>
        [Parameter(NAME + " " + VERSION, Group = "Identity", DefaultValue = "https://ctrader.guru/product/engulfing/")]
        public string ProductInfo { get; set; }

        /// <summary>
        /// Equivalente al magic number per Metatrader
        /// </summary>
        [Parameter("Label ( Magic Name )", Group = "Identity", DefaultValue = NAME)]
        public string MyLabel { get; set; }

        /// <summary>
        /// Riferimento per il preset
        /// </summary>
        [Parameter("Preset information", Group = "Identity", DefaultValue = "EURUSD 1H")]
        public string PresetInfo { get; set; }
        /*
        [Parameter("Stop Loss", DefaultValue = 5, MinValue = 0, Step = 0.1)]
        public double SL { get; set; }

        [Parameter("Take Profit", DefaultValue = 10, MinValue = 0, Step = 0.1)]
        public double TP { get; set; }
        */

        /// <summary>
        /// Rischio : Ricompensa
        /// </summary>
        [Parameter("R:R 1:", Group = "Money Management", DefaultValue = 2.5, MinValue = 0)]
        public double AutoRR { get; set; }

        /// <summary>
        /// Il numero di pips per l'attivazione del breakeven, zero disabilita il controllo
        /// </summary>
        [Parameter("Break Even From ( 0 = disabled )", Group = "Money Management", DefaultValue = 15, MinValue = 0, Step = 0.1)]
        public double BEfrom { get; set; }

        /// <summary>
        /// Il numero di pips da spostare per lo stoploss
        /// </summary>
        [Parameter("Break Even To", Group = "Money Management", DefaultValue = 1.5, MinValue = 1, Step = 0.1)]
        public double BEto { get; set; }

        /// <summary>
        /// Massimo spread consentito
        /// </summary>
        [Parameter("Max Spread allowed", Group = "Money Management", DefaultValue = 0.7, MinValue = 0.1, Step = 0.1)]
        public double SpreadToTrigger { get; set; }

        /// <summary>
        /// Il numero di pips dello slittamento dell'ordine, il broker dovrebbe rispettarlo
        /// </summary>
        [Parameter("Slippage", Group = "Money Management", DefaultValue = 2.0, MinValue = 0.5, Step = 0.1)]
        public double Slippage { get; set; }

        /// <summary>
        /// Quale capitale prendere in considerazione per il calcolo della size
        /// </summary>
        [Parameter("Capital", Group = "Money Management", DefaultValue = AccCapital.Balance)]
        public AccCapital MyCapital { get; set; }

        /// <summary>
        /// Percentuale di rischio sul capitale
        /// </summary>
        [Parameter("% Risk", Group = "Money Management", DefaultValue = 3, MinValue = 0.1, Step = 0.1)]
        public double MyRisk { get; set; }
        /*
        [Parameter("Pips To Calculate ( if no stoploss )", DefaultValue = 20, MinValue = 0, Step = 0.1)]
        public double fakeSL { get; set; }
        */

        /// <summary>
        /// Minimi lotti consentiti
        /// </summary>
        [Parameter("Minimum Lots", Group = "Money Management", DefaultValue = 0.01, MinValue = 0.01, Step = 0.01)]
        public double MinLots { get; set; }

        /// <summary>
        /// Massimi lotti consentiti
        /// </summary>
        [Parameter("Maximum Lots", Group = "Money Management", DefaultValue = 1, MinValue = 0.01, Step = 0.01)]
        public double MaxLots { get; set; }

        /// <summary>
        /// Oltre questo orario entra in pausa
        /// </summary>
        [Parameter("Pause over this time", Group = "Filters", DefaultValue = 0, MinValue = 0, MaxValue = 23.59)]
        public double PauseOver { get; set; }

        /// <summary>
        /// Sotto questo orario rimane in pausa
        /// </summary>
        [Parameter("Pause under this time", Group = "Filters", DefaultValue = 0, MinValue = 0, MaxValue = 23.59)]
        public double PauseUnder { get; set; }

        /// <summary>
        /// Massimo GAP consentito
        /// </summary>
        [Parameter("Max GAP Allowed", Group = "Filters", DefaultValue = 3, MinValue = 0.1)]
        public double GAP { get; set; }

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

        #endregion

        #region Property

        /// <summary>
        /// Registra l'apertura della posizione nella candela corrente
        /// </summary>
        bool openedInThisBar = false;

        // --> I tre indicatori con cui analizzeremo i filtri e i trigger
        ExponentialMovingAverage EMAfast;
        ExponentialMovingAverage EMAslow;
        ParabolicSAR SAR;

        #endregion

        #region cBot Events

        /// <summary>
        /// Eseguita una sola volta, alla partenza del cbot, inizializziamo le informazioni che ci interessano
        /// </summary>
        protected override void OnStart()
        {

            // --> Stampo nei log la versione corrente
            Print("{0} : {1}", NAME, VERSION);

            // --> Se viene settato l'ID effettua un controllo per verificare eventuali aggiornamenti
            _checkProductUpdate();

            // --> Fissiamo la logica degli indicatori 
            EMAfast = Indicators.ExponentialMovingAverage(Bars.ClosePrices, EmaFastPeriod);
            EMAslow = Indicators.ExponentialMovingAverage(Bars.ClosePrices, EmaSlowPeriod);
            SAR = Indicators.ParabolicSAR(0.02, 0.2);


        }

        /// <summary>
        /// Eseguita ad ogni cambio di candela, viene utilizzata per analizzare l'ingresso
        /// </summary>
        protected override void OnBar()
        {

            // --> Resetto il flag ad ogni nuova candela
            openedInThisBar = false;

            // --> Condizione condivisa, non voglio uno spread troppo alto e voglio aprire una sola operazione per volta
            bool sharedCondition = (!openedInThisBar && !_iAmInGAP() && !_iAmInPause() && _getSpreadInformation() <= SpreadToTrigger && Positions.FindAll(MyLabel, SymbolName).Length == 0);

            // --> Analizzo la presenza di eventuali trigger d'ingresso
            bool triggerBuy = _calculateLongTrigger(_calculateLongFilter(sharedCondition));
            bool triggerSell = _calculateShortTrigger(_calculateShortFilter(sharedCondition));

            // --> Se ho entrambi i trigger qualcosa non va, lo segno nei log e fermo la routin
            if (triggerBuy && triggerSell)
            {

                Print("{0} {1} ERROR : trigger buy and sell !", MyLabel, SymbolName);
                return;

            }

            double tmpSL = 0;
            double tmpTP = 0;

            // --> Calcolo la size automaticamente
            _calculateSLTP(ref tmpSL, ref tmpTP);

            // --> In caso di trigger d'ingresso devo essere sicuro di avere stoploss e takeprofit
            if ((triggerBuy || triggerSell) && (tmpSL == 0 || tmpTP == 0))
            {

                Print("Stoploss or Takeprofit at zero");
                return;

            }

            var volumeInUnits = Symbol.QuantityToVolumeInUnits(_calculateSize(tmpSL));

            if (triggerBuy)
            {

                ExecuteMarketRangeOrder(TradeType.Buy, SymbolName, volumeInUnits, Slippage, Symbol.Ask, MyLabel, tmpSL, tmpTP);
                openedInThisBar = true;

            }
            else if (triggerSell)
            {

                ExecuteMarketRangeOrder(TradeType.Sell, Symbol.Name, volumeInUnits, Slippage, Symbol.Bid, MyLabel, tmpSL, tmpTP);
                openedInThisBar = true;

            }

        }

        /// <summary>
        /// Eseguita ad ogni tick, utilizzata in engulfing per controllare la modifica del breakeven
        /// </summary>
        protected override void OnTick()
        {

            // --> Se abilitato per mezzo dell'attivazione controllo la modifica del breakeven
            if (BEfrom > 0)
                _checkBE(BEfrom);

        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Calcola la condizione favolrevole del filtro in ottica long
        /// </summary>
        /// <param name="condition">Condizione condivisa</param>
        /// <returns>La condizione del filtro long soddisfatta</returns>
        private bool _calculateLongFilter(bool condition = true)
        {

            // --> Se la condizione condivisa non è presente è inutile continuare
            if (!condition)
                return false;

            // --> Devo partire dal basso, la deviazione deve ancora iniziare
            return (SAR.Result.Last(2) > Bars.HighPrices.Last(2) && SAR.Result.Last(1) > Bars.HighPrices.Last(1));

        }

        /// <summary>
        /// Calcola la condizione favolrevole del filtro in ottica short 
        /// </summary>
        /// <param name="condition">Condizione condivisa</param>
        /// <returns>La condizione del filtro short soddisfatta</returns>
        private bool _calculateShortFilter(bool condition = true)
        {

            // --> Se la condizione condivisa non è presente è inutile continuare
            if (!condition)
                return false;

            // --> Devo partire dall'alto, la deviazione deve ancora iniziare
            return (SAR.Result.Last(2) < Bars.LowPrices.Last(2) && SAR.Result.Last(1) < Bars.LowPrices.Last(1));

        }

        /// <summary>
        /// In base alla distanza dello stoploss calcola il takeprofit R:R
        /// </summary>
        /// <param name="SL">Stoploss di riferimento</param>
        /// <param name="TP">Takeprofit di riferimento</param>
        private void _calculateSLTP(ref double SL, ref double TP)
        {

            // --> Indipendentemente dalla direzione long/short il pattern offre le stesse condizioni di calcolo
            double husbandShadow = (Bars.HighPrices.Last(1) - Bars.LowPrices.Last(1)) / Symbol.PipSize;

            if (AutoRR > 0 && husbandShadow > 0)
            {

                SL = husbandShadow;
                TP = SL * AutoRR;

            }

        }

        /// <summary>
        /// Calcola la condizione d'ingresso in ottica long
        /// </summary>
        /// <param name="filter">Condizione filtro</param>
        /// <returns>La condizione del trigger long soddisfatta</returns>
        private bool _calculateLongTrigger(bool filter = true)
        {

            // --> Se il filtro non è soddisfatto è inutile proseguire
            if (!filter)
                return false;

            // --> Restituisco la condizione
            return _haveEngulfingBullish();

        }

        /// <summary>
        /// Calcola la condizione d'ingresso in ottica short
        /// </summary>
        /// <param name="filter">Condizione filtro</param>
        /// <returns>La condizione del trigger short soddisfatta</returns>
        private bool _calculateShortTrigger(bool filter = true)
        {

            // --> Se il filtro non è soddisfatto è inutile proseguire
            if (!filter)
                return false;

            // --> Restituisco la condizione
            return _haveEngulfingBearish();

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

        /// <summary>
        /// Questa funzione potrebbe essere cancellata ma la lascio a beneficio di archivio e per istruire chi si avvicina al C#
        /// </summary>
        /// <returns>Lo spread del simbolo corrente</returns>
        [ObsoleteAttribute("Obsoleta, utilizza la nuova proprieta 'Symbol.Spread;'")]
        private double _getSpreadInformation()
        {

            // --> Restituisco lo spread corrente
            return Symbol.Spread;

            // --> Prima che venisse fornita nelle API
            // --> return Math.Round(Symbol.Spread / Symbol.PipSize, 2);

        }

        /// <summary>
        /// Controlla e modifica le posizioni aperte con questo cBot in caso di breakeven
        /// </summary>
        /// <param name="beFrom">Il numero di pips per l'attivazione</param>
        private void _checkBE(double beFrom)
        {

            var MyPositions = Positions.FindAll(MyLabel, SymbolName);

            foreach (var position in MyPositions)
            {

                if (position.TradeType == TradeType.Buy)
                {

                    if ((Symbol.Bid >= (position.EntryPrice + (beFrom * Symbol.PipSize))) && (position.StopLoss == null || position.StopLoss < position.EntryPrice))
                    {

                        ModifyPosition(position, (position.EntryPrice + (BEto * Symbol.PipSize)), position.TakeProfit);

                    }

                }
                else if (position.TradeType == TradeType.Sell)
                {

                    if ((Symbol.Ask <= (position.EntryPrice - (beFrom * Symbol.PipSize))) && (position.StopLoss == null || position.StopLoss > position.EntryPrice))
                    {

                        ModifyPosition(position, (position.EntryPrice - (BEto * Symbol.PipSize)), position.TakeProfit);

                    }

                }

            }

        }

        /// <summary>
        /// Decide come calcolare la size
        /// </summary>
        /// <param name="mySL"></param>
        /// <returns>La size calcolata</returns>
        private double _calculateSize(double mySL)
        {

            if (mySL > 0)
                return _getLotSize(_getMyCapital(MyCapital), mySL, MyRisk, MinLots, MaxLots);
            /*
            if (fakeSL > 0)return _getLotSize(_getMyCapital(MyCapital), fakeSL, MyRisk, MinLots, MaxLots);
            */
            return MinLots;

        }

        /// <summary>
        /// Calcola la size da utilizzare
        /// </summary>
        /// <param name="capital">Il capitale con cui lavorare</param>
        /// <param name="stoploss">Lo stoploss</param>
        /// <param name="percentage">Il rischio</param>
        /// <param name="Minim">Minimi lotti consentiti</param>
        /// <param name="Maxi">Massimi lotti consentiti</param>
        /// <returns>Restituisce i lotti calcolati</returns>
        private double _getLotSize(double capital, double stoploss, double percentage, double Minim, double Maxi)
        {

            // --> Ottengo la percentuale di rischio
            double moneyrisk = ((capital / 100) * percentage);

            // --> Converto i pips per la coppia corrente
            double sl_double = (stoploss * Symbol.PipSize);

            // --> In formato 0.01 = microlotto double lots = Math.Round(Symbol.VolumeInUnitsToQuantity(moneyrisk / ((sl_double * Symbol.TickValue) / Symbol.TickSize)), 2);

            // --> In formato volume 1K = 1000 Math.Round((moneyrisk / ((sl_double * Symbol.TickValue) / Symbol.TickSize)), 2);

            double lots = Math.Round(Symbol.VolumeInUnitsToQuantity(moneyrisk / ((sl_double * Symbol.TickValue) / Symbol.TickSize)), 2);

            if (lots < Minim)
                return Minim;
            if (lots > Maxi)
                return Maxi;

            return lots;

        }

        /// <summary>
        /// Restituisco la scelta fatta dall'utente per il capitale
        /// </summary>
        /// <param name="x">Enumeratore per il capitale</param>
        /// <returns>Il capitale sul quale effettuare i calcoli</returns>
        private double _getMyCapital(AccCapital x)
        {

            switch (x)
            {

                case AccCapital.Equity:

                    return Account.Equity;

                case AccCapital.FreeMargin:

                    return Account.FreeMargin;
                default:


                    return Account.Balance;

            }

        }

        /// <summary>
        /// Controlla se le condizioni di GAP siano soddisfatte
        /// </summary>
        /// <returns>Condizione di GAP soddisfatta o meno</returns>
        private bool _iAmInGAP()
        {

            double K = 0;

            if (Bars.ClosePrices.Last(1) > Bars.OpenPrices.Last(0))
            {

                K = Math.Round(((Bars.ClosePrices.Last(1) - Bars.OpenPrices.Last(0)) / Symbol.PipSize), 2);

            }
            else if (Bars.OpenPrices.Last(0) > Bars.ClosePrices.Last(1))
            {

                K = Math.Round(((Bars.OpenPrices.Last(0) - Bars.ClosePrices.Last(1)) / Symbol.PipSize), 2);

            }

            return (K > GAP);

        }

        /// <summary>
        /// Controlla la presenza di una fascia oraria in pausa
        /// </summary>
        /// <returns></returns>
        private bool _iAmInPause()
        {

            // --> Controllo disabilitato
            if (PauseUnder == 0 && PauseOver == 0)
                return false;

            // --> Utilizzo una logica long quindi devo tradurla in time
            string nowHour = (Server.Time.Hour < 10) ? string.Format("0{0}", Server.Time.Hour) : string.Format("{0}", Server.Time.Hour);
            string nowMinute = (Server.Time.Minute < 10) ? string.Format("0{0}", Server.Time.Minute) : string.Format("{0}", Server.Time.Minute);

            double adesso = Convert.ToDouble(string.Format("{0},{1}", nowHour, nowMinute));

            if (PauseOver < PauseUnder && adesso >= PauseOver && adesso <= PauseUnder)
            {

                return true;

            }
            else if (PauseOver > PauseUnder && ((adesso >= PauseOver && adesso <= 23.59) || adesso <= PauseUnder))
            {

                return true;

            }

            return false;

        }

        /// <summary>
        /// Effettua un controllo sul sito ctrader.guru per mezzo delle API per verificare la presenza di aggiornamenti, solo in realtime
        /// </summary>
        private void _checkProductUpdate()
        {

            // --> Controllo solo se solo in realtime, evito le chiamate in backtest
            if (RunningMode != RunningMode.RealTime)
                return;

            // --> Organizzo i dati per la richiesta degli aggiornamenti
            Guru.API.RequestProductInfo Request = new Guru.API.RequestProductInfo 
            {

                MyProduct = new Guru.Product 
                {

                    ID = ID,
                    Name = NAME,
                    Version = VERSION

                },
                AccountBroker = Account.BrokerName,
                AccountNumber = Account.Number

            };

            // --> Effettuo la richiesta
            Guru.API Response = new Guru.API(Request);

            // --> Controllo per prima cosa la presenza di errori di comunicazioni
            if (Response.ProductInfo.Exception != "")
            {

                Print("{0} Exception : {1}", NAME, Response.ProductInfo.Exception);

            }
            // --> Chiedo conferma della presenza di nuovi aggiornamenti
            else if (Response.HaveNewUpdate())
            {

                string updatemex = string.Format("{0} : Updates available {1} ( {2} )", NAME, Response.ProductInfo.LastProduct.Version, Response.ProductInfo.LastProduct.Updated);

                // --> Informo l'utente con un messaggio sul grafico e nei log del cbot
                Chart.DrawStaticText(NAME + "Updates", updatemex, VerticalAlignment.Top, HorizontalAlignment.Left, Color.Red);
                Print(updatemex);

            }

        }

        #endregion

    }

}

/// <summary>
/// NameSpace che racchiude tutte le feature ctrader.guru
/// </summary>
namespace Guru
{
    /// <summary>
    /// Classe che definisce lo standard identificativo del prodotto nel marketplace ctrader.guru
    /// </summary>
    public class Product
    {

        public int ID = 0;
        public string Name = "";
        public string Version = "";
        public string Updated = "";

    }

    /// <summary>
    /// Offre la possibilità di utilizzare le API messe a disposizione da ctrader.guru per verificare gli aggiornamenti del prodotto.
    /// Permessi utente "AccessRights = AccessRights.FullAccess" per accedere a internet ed utilizzare JSON
    /// </summary>
    public class API
    {
        /// <summary>
        /// Costante da non modificare, corrisponde alla pagina dei servizi API
        /// </summary>
        private const string Service = "https://ctrader.guru/api/product_info/";

        /// <summary>
        /// Costante da non modificare, utilizzata per filtrare le richieste
        /// </summary>
        private const string UserAgent = "cTrader Guru";

        /// <summary>
        /// Variabile dove verranno inserite le direttive per la richiesta
        /// </summary>
        private RequestProductInfo RequestProduct = new RequestProductInfo();

        /// <summary>
        /// Variabile dove verranno inserite le informazioni identificative dal server dopo l'inizializzazione della classe API
        /// </summary>
        public ResponseProductInfo ProductInfo = new ResponseProductInfo();

        /// <summary>
        /// Classe che formalizza i parametri di richiesta, vengono inviate le informazioni del prodotto e di profilazione a fini statistici
        /// </summary>
        public class RequestProductInfo
        {

            /// <summary>
            /// Il prodotto corrente per il quale richiediamo le informazioni
            /// </summary>
            public Product MyProduct = new Product();

            /// <summary>
            /// Broker con il quale effettiamo la richiesta
            /// </summary>
            public string AccountBroker = "";

            /// <summary>
            /// Il numero di conto con il quale chiediamo le informazioni
            /// </summary>
            public int AccountNumber = 0;

        }

        /// <summary>
        /// Classe che formalizza lo standard per identificare le informazioni del prodotto
        /// </summary>
        public class ResponseProductInfo
        {

            /// <summary>
            /// Il prodotto corrente per il quale vengono fornite le informazioni
            /// </summary>
            public Product LastProduct = new Product();

            /// <summary>
            /// Eccezioni in fase di richiesta al server, da utilizzare per controllare l'esito della comunicazione
            /// </summary>
            public string Exception = "";

            /// <summary>
            /// La risposta del server
            /// </summary>
            public string Source = "";

        }

        /// <summary>
        /// Richiede le informazioni del prodotto richiesto
        /// </summary>
        /// <param name="Request"></param>
        public API(RequestProductInfo Request)
        {

            RequestProduct = Request;

            // --> Non controllo se non ho l'ID del prodotto
            if (Request.MyProduct.ID <= 0)
                return;

            // --> Dobbiamo supervisionare la chiamata per registrare l'eccexione
            try
            {

                // --> Strutturo le informazioni per la richiesta POST
                NameValueCollection data = new NameValueCollection 
                {
                    {
                        "account_broker",
                        Request.AccountBroker
                    },
                    {
                        "account_number",
                        Request.AccountNumber.ToString()
                    },
                    {
                        "my_version",
                        Request.MyProduct.Version
                    },
                    {
                        "productid",
                        Request.MyProduct.ID.ToString()
                    }
                };

                // --> Autorizzo tutte le pagine di questo dominio
                Uri myuri = new Uri(Service);
                string pattern = string.Format("{0}://{1}/.*", myuri.Scheme, myuri.Host);

                Regex urlRegEx = new Regex(pattern);
                WebPermission p = new WebPermission(NetworkAccess.Connect, urlRegEx);
                p.Assert();

                // --> Protocollo di sicurezza https://
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;

                // -->> Richiedo le informazioni al server
                using (var wb = new WebClient())
                {

                    wb.Headers.Add("User-Agent", UserAgent);

                    var response = wb.UploadValues(myuri, "POST", data);
                    ProductInfo.Source = Encoding.UTF8.GetString(response);

                }

                // -->>> Nel cBot necessita l'attivazione di "AccessRights = AccessRights.FullAccess"
                ProductInfo.LastProduct = JsonConvert.DeserializeObject<Product>(ProductInfo.Source);

            } catch (Exception Exp)
            {

                // --> Qualcosa è andato storto, registro l'eccezione
                ProductInfo.Exception = Exp.Message;

            }

        }

        /// <summary>
        /// Esegue un confronto tra le versioni per determinare la presenza di aggiornamenti
        /// </summary>
        /// <returns></returns>
        public bool HaveNewUpdate()
        {

            // --> Voglio essere sicuro che stiamo lavorando con le informazioni giuste
            return (ProductInfo.LastProduct.ID == RequestProduct.MyProduct.ID && ProductInfo.LastProduct.Version != "" && RequestProduct.MyProduct.Version != "" && new Version(RequestProduct.MyProduct.Version).CompareTo(new Version(ProductInfo.LastProduct.Version)) < 0);

        }

    }

}

