# Finance Library

### Installation

 * #### Classic
    If you would like to use this library in your own project, use the standard installation process. of packages from NuGet.
    
    ```bash
    Install-Package CosminSanda.Finance -Version 0.0.3
    ```

 * #### Notebook
    If you would like to use this in a .NET enabled Jupyter notebook use this command in a cell:
 
    ```
    #r "nuget,CosminSanda.Finance"
    ```
   
### Usage

##### Get a list of dates when the earnings for a symbol are released

```c#
var earnings = await EarningsCalendar.GetEarnings("ZTO");
```

##### Get a list of financial stats in a selected date interval for a symbol

```c#
var earnings = await Quotes.GetQuotes("ZTO", "2020-01-01", "2020-01-15");
```

##### Get a list of financial stats around an earning date for a symbol

The method will take into account if the earnings date is after or before market close.
If not specified, it will be considered to be after market close.

```c#
var quotes = await Quotes.GetQuotesAround("ZTO", earningsDate: earnings[6], lookAround: 3);
```