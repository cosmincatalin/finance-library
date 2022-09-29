# CosminSanda.Finance

![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/CosminSanda.Finance?style=plastic)
![GitHub Workflow Status](https://img.shields.io/github/workflow/status/cosmincatalin/finance-library/Test)
[![Coverage Status](https://coveralls.io/repos/github/cosmincatalin/finance-library/badge.svg?branch=master)](https://coveralls.io/github/cosmincatalin/finance-library?branch=master)

A small library that can be used to easily scrape information about Earnings Releases and financial quotes from Yahoo Finance.

## Installation

 * #### Classic
    If you would like to use this library in your own project, use the standard installation process. of packages from NuGet.

    ```bash
    Install-Package CosminSanda.Finance
    ```

 * #### Notebook
    If you would like to use this in a .NET enabled Jupyter notebook use this command in a cell:

    ```
    #r "nuget,CosminSanda.Finance"
    ```

## Usage

### Get a list of dates when the earnings for a symbol are released

```c#
var earnings = await EarningsCalendar.GetEarnings("ZTO");
```

### Get a list of financial stats in a selected date interval for a symbol

```c#
var earnings = await Quotes.GetQuotes("ZTO", "2020-01-01", "2020-01-15");
```

### Get a list of financial stats around an earning date for a symbol

The method will take into account if the earnings date is after or before market close.
If not specified, it will be considered to be after market close.

```c#
var quotes = await Quotes.GetQuotesAround("ZTO", earningsDate: earnings[6], lookAround: 3);
```

### Get the next earnings date for a symbol

```c#
var nextEarnings = await EarningsCalendar.GetNextEarningsDate("ZTO");
```