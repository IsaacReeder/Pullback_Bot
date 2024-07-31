

     ______       _ _  _                 _             ______             
    (_____ \     | | || |               | |           (____  \        _   
     _____) )   _| | || |__  _____  ____| |  _         ____)  ) ___ _| |_ 
    |  ____/ | | | | ||  _ \(____ |/ ___) |_/ )       |  __  ( / _ (_   _)
    | |    | |_| | | || |_) ) ___ ( (___|  _ ( _______| |__)  ) |_| || |_ 
    |_|    |____/ \_)_)____/\_____|\____)_| \_|_______)______/ \___/  \__)
    
        
Key Components and Explanations:
1. **Parameters**:

- **Volume**: The volume of each trade.
- **TakeProfitPips and StopLossPips**: These set the take profit and stop loss levels in pips.
- **RiskPercentage**: Determines how much of the account balance you're willing to risk per trade.
- **FastEmaPeriod and SlowEmaPeriod**: Periods for the fast and slow exponential moving averages (EMAs).

2. **OnStart()**: Initializes the EMAs when the bot starts.

3. **OnBar()**: This method is triggered on every new bar. It checks the current trend using EMAs and places trades based on pullback conditions.

4. **IsTradeAllowed()**: Ensures that a new trade is only placed if no existing position of the same type is already open.

5. **CalculateVolume()**: Computes the trading volume based on the account risk and stop loss distance.
