# CosminSanda.Finance

## Description

This package simplifies the process of retrieving data from Yahoo Finance.  
Currently, it can fetch information about earnings calls and about daily quotes.  

This package can be used, typically, to analyze the price action around historical earnings calls for the purpose
of establishing strategies for future earnings releases a.k.a **"Playing the Earnings"**.  

The package acts as a proxy to Yahoo Finance and is essentially a web scraper.  
While in previous versions, caching was built in, it has been removed.  
That means all methods make requests directly to Yahoo Finance and it is your responsibility to cache the data so as to avoid redundant requests.  
The reason for disabling the cache has to do with the lack of guarantees regarding the provided data which can lead to inconsistencies for less popular instruments.  

* Methods are made static to make it easier to use from interactive environments.  
That is also the reason why there aren't a lot of options for Dependencies Injection of, for example, logging.  
* To further simplify the use of methods in the package, arguments are passed using basic types like `string` and `int32`.
