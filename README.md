# IslandLogin
This solution runs on port 5001 and demostrates how to call a REST service on port 8001.
See the *OidcDownstreamTokenVerifier* for implementation details of web system using the
JWT token obtained here from island.is for login. Here we also have the */WeatherForecast/GetWeatherForecast* 
with JWT Authorize Bearer header authentication. Note that for this call there is no state created so each call
has to supply Authorize Bearer header to gain access.

### ClientSecret
This is naturally not included in this git repository. To configure your own
update *ClientId* and *ClientSecret* in *appsettings.json* appropriately. 
Note that island.is assigns the same value to *Audience* as the *CLientId*. 
Make sure these match in the configuration files.

The existence of appsettings.secret.json allows you to set the values mentioned without them ever
going to a git repository as the file is listed in *.gitignore*.

### Possible handling of custom Lifetime logic for login / token validity
The class *AccessTokenExpirationMiddleware* has some initial thoughts on this.

Please note that commented sections of code are NOT proper implementations of this function,
more work is needed once we have a valid requirement.

### Questions
If you have questions email tolli@kopavogur.is or use the same handle on Teams.
