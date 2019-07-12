using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using SocialWeb.Models;
//using SocialWeb.Services;
using System.Configuration;

namespace SocialWeb.Common
{
    public class CommonAuthen : AuthorizeAttribute
    {

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Session[Constants.LoginSession] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                                new RouteValueDictionary
                               {
                                   {"action", "Login" },
                                   {"controller", "Account" }
                               });
            }
            //base.HandleUnauthorizedRequest(filterContext);
        }

        //public void InitUser(HttpContextBase HttpContext)
        //{
        //    LoginModel user = new LoginModel
        //    {
        //        apiTokenUrl = "https://api-sit.ocb.com.vn/ocbsit/developerhub/ocb-sit-oauth/oauth2/token",
        //        apiUrl = "https://api-sit.ocb.com.vn/ocbsit/developerhub/",
        //        Certificate = "MIIEOzCCAyOgAwIBAgIJAJTFvdpeKAEDMA0GCSqGSIb3DQEBCwUAMIGuMQswCQYDVQQGEwJWTjEPMA0GA1UECAwGSGEgTm9pMQ8wDQYDVQQHDAZIYSBOb2kxEjAQBgNVBAoMCVNlYXRlY2hpdDEnMCUGA1UECwweQ09ORyBOR0hFIFRIT05HIFRJTiBEQU5HIE5BTSBBMRkwFwYDVQQDDBBzZWF0ZWNoaXQuY29tLnZuMSUwIwYJKoZIhvcNAQkBFhZ0YW5kbkBzZWF0ZWNoaXQuY29tLnZuMB4XDTE5MDYwNTA5NDAwOVoXDTI0MDYwMzA5NDAwOVowgZQxCzAJBgNVBAYTAlZOMQwwCgYDVQQIDANIQ00xDDAKBgNVBAcMA0hDTTEMMAoGA1UECgwDT0NCMSMwIQYDVQQLDBpOR0FOIEhBTkcgQ1BUTSBQSFVPTkcgRE9ORzETMBEGA1UEAwwKb2NiLmNvbS52bjEhMB8GCSqGSIb3DQEJARYSdGFuZG5nb2NAZ21haWwuY29tMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAo//zbAPzdwZow0+x6tf/O6ilctGFXm7XZmOPJwKSXEgMc/vud8sRhztK9uBhFBmDykA9Vky0twmoLLqkSz0aU351OAyEiNttX2ykYzJWsUXn9R9qK7BkMlBX8ork+BPGNgHjdJKJ77fjL8VEXStfINx/QT3Ox+QQLAstMSfIPPHAmWFNvki64uUQlffCClbNMVXPhwZncqTBh/ZMv3HPXixgORDNwyv5n9oxd8CTb1y0ezcg3Nv7xcSwSJPfyFw0IEM1wYQAg13KyIKv4wIfH/y3V4bslgME6xXQGwy/pwTwwR7nSKsdOC5JFlorPAgjLjtqlse9ub0/Cr2bAjtRUwIDAQABo3QwcjAfBgNVHSMEGDAWgBSMiEw3M7yeSzYZ6fF4W9SamxzuWzAJBgNVHRMEAjAAMAsGA1UdDwQEAwIE8DA3BgNVHREEMDAughJhcGktc2l0Lm9jYi5jb20udm6CGGFwaS1nd2FkbS1zaXQub2NiLmNvbS52bjANBgkqhkiG9w0BAQsFAAOCAQEARgrxqLK5YN7DtWAush5RY/ebDCekxu/8O90nwnaIiFdY9ShdfAFmvtCs1X+siBDpxx3Co2bhYKuaXT42X0Wuu/fVWOEpD2+18w3Sj22n6uZTq4vCZfvdX7N1nuSY6YBHvvqu6/A1eS0IEalEqYobp2CyF4AazP5O3u7dqQaElusqmddioDvVDiJcMbNkdOuQec4NMX6/u4lQaYk4RF0JoCF1Sa22lsWOCIc/HEGaeAlaLgRCFhPA+Vk0f54tOi+yF9zRnjYZO8HL9sT6d0UETcNuh5QUvfcK4Rhhq/oHsN6rvkA8Y3MYkvxWal1WpyTPlnUdfjrepzKID6jMYIFxIA==",
        //        PrivateKey = "MIIEpAIBAAKCAQEAo//zbAPzdwZow0+x6tf/O6ilctGFXm7XZmOPJwKSXEgMc/vud8sRhztK9uBhFBmDykA9Vky0twmoLLqkSz0aU351OAyEiNttX2ykYzJWsUXn9R9qK7BkMlBX8ork+BPGNgHjdJKJ77fjL8VEXStfINx/QT3Ox+QQLAstMSfIPPHAmWFNvki64uUQlffCClbNMVXPhwZncqTBh/ZMv3HPXixgORDNwyv5n9oxd8CTb1y0ezcg3Nv7xcSwSJPfyFw0IEM1wYQAg13KyIKv4wIfH/y3V4bslgME6xXQGwy/pwTwwR7nSKsdOC5JFlorPAgjLjtqlse9ub0/Cr2bAjtRUwIDAQABAoIBAQCVRKdIjygQE7NS4bysZcCXil5cbTuYwgYn2UI4XWzdtW4wOwPH4PqpPVxz67IwWzDK60FoxRRO7Ok3HQHgwVKu4BDM3QfckOuxyO6uouipHVmMj/VQopHwAZSq26Sf70+fZISkW6RUneiYWFJrAsjo3gitVxZYdcoKbHnLncvxOxvMW6BDO9vjZ7CNrRs4YSYtUrfn+RIemYOPJf0BzmZPJFCdYp5Uf8tYChrj/vcMjIsXAZ/dtgid01ZC9K/RMloqzJsrbNnIfQbjmxj3oQuDdW+ozOMnlZwKzjaDoj4S92HudRz3J1X03XYKTnJ4qeQzHWhMkOBSKuBcFY8HwJmBAoGBANMANK6FRelCYrY1HbFgIHCQ02uFgal9YX1Smpd0xNF8aVZcM8cCUXt18rZ8UdHanZeTXpdHbIraekTctg4hmYzUjEgbf9Ubp+DsYzMGm8O4l3A3hx9b9PpC3zeM6Fj9uoPtKFI5Cej7t8YS55N81VckUzatrFEnOu9OaJWK6P8zAoGBAMb5rhL98IOkL0h1eqyEFVJpJH3UlruBLrD7qZxH0ez1FsmvmQnY3ot8YDx+mtYHXv0qiSvqeLWy0hjbOTYyqRhIYYGktiZaMmSH+KvPHNgIYcD7iNpiE3NmCLza6K3QARN5tnvgTuA4kY3fWjIFKzShuS926JJ9Qo07TFSFruVhAoGBAJz12jq5CXir2aKRgLUiPP9/vMaPWhUrIAqKGFXylzb+xZ1omVvBbbvZ0ePON09UwUawaf0/NI9WVv5C8Wsxs3f/5Rr+2ek92XSIZILgt56xAnaH2AyL64D/ne1E9NK+bLEXCpeftq+KEPtXtM0SX+GjNAPIzhbQiBbczQ/xdcHhAoGAB8Z//+v+dxZ2Zo14sr8imirTqzsgfMlKis36zcmcsXbOYilDgLgB0k+U7yg/Yre9BYWhAJ9UAj2vqhr+/Fg0dWd2r/tAxvTlXTpXBFe+l86UC1eI/IeynOLS2pZvW0Nyl1E9SU/1pRtwzKt6udOr4Y2kT++EnRzZ+ezkSbVDpWECgYBkPKnepwJ1xHQ5cB9lj0LbX6tGnkTnOf/dxkAJfM3d65xCKMhuSYR0drgzYzcjbLMx7L8fKp3yroQVCQh8vCziFsnNx1gf8tjInFnL7q0JxxnipehWnmVIs7XtoXtpK8e+ZFRS/xMelhxBmaqAao8iV6Kf8q8WDSnhOPXPb2hbAg==",
        //        ClientId = "ce17dfaf99287e50bf22266ba7a80732",
        //        ClientSecret = "a009e24f7dcc0d20596adece715f0c18",
        //        Scope = "OCBSIT",
        //        Username = "jka3375",
        //        Password = "hpp4481"

        //    };
        //    var loginresult = ServiceAPI.Current.LoginAccess(user);
        //    if (loginresult.IsOk)
        //    {
        //        user.Authentication = ((tokenresponse)loginresult.dataObj).access_token;
        //        HttpContext.Session[Constants.LoginSession] = user;
        //    }
        //}

    }
}