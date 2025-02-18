using HttpProxyAuthentication;
using HttpProxyAuthentication.Types;

namespace ProxyCallTest
{
    [TestClass]
    public sealed class PermissionInvokeTest
    {
        [TestMethod]
        public async ValueTask TestSm2_Sm2KeyCall()
        {
            //string key = "MIIDDgIBATBHBgoqgRzPVQYBBAIBBgcqgRzPVQFoBDAj2kNIPj4ZbedlrBeMpTc9EBxqikulhk8vmaXlk5FtUSie3Dbnw2dwEj61qnfXPB8wggK+BgoqgRzPVQYBBAIBBIICrjCCAqowggJOoAMCAQICBROHKHhIMAwGCCqBHM9VAYN1BQAwJTELMAkGA1UEBhMCQ04xFjAUBgNVBAoMDUNGQ0EgU00yIE9DQTEwHhcNMjIwOTA5MDI0NTAzWhcNMjcwOTA5MDI0NTAzWjCBhzELMAkGA1UEBhMCQ04xEjAQBgNVBAoMCUNGQ0EgT0NBMTEPMA0GA1UECwwGQmFvZm9vMRkwFwYDVQQLDBBPcmdhbml6YXRpb25hbC0yMTgwNgYDVQQDDC9CYW9mb29A5a6d5LuY5rWL6K+V5ZWG5oi3QE45MTMxMDEwNDMxMjMwOTI3N0NAMTBZMBMGByqGSM49AgEGCCqBHM9VAYItA0IABB4pjqbSHflMUv6yxBBY5NbEsTRwYv+mIQsKHvXdTljT4RGteQAAuZ+L7P3KRqTYpMkdlYU626oa0P02LP8FuUWjggEEMIIBADAfBgNVHSMEGDAWgBRck1ggWiRzVhAbZFAQ7OmnygdBETAMBgNVHRMBAf8EAjAAMEgGA1UdIARBMD8wPQYIYIEchu8qAQEwMTAvBggrBgEFBQcCARYjaHR0cDovL3d3dy5jZmNhLmNvbS5jbi91cy91cy0xNC5odG0wNwYDVR0fBDAwLjAsoCqgKIYmaHR0cDovL2NybC5jZmNhLmNvbS5jbi9TTTIvY3JsNzA2NC5jcmwwDgYDVR0PAQH/BAQDAgbAMB0GA1UdDgQWBBQxbHO/sbJkM+nWGetfAM5uF5M+5TAdBgNVHSUEFjAUBggrBgEFBQcDAgYIKwYBBQUHAwQwDAYIKoEcz1UBg3UFAANIADBFAiEA7oSvjEhghT9FxCfihv7JYIQCtwmKC7D+UyatiezU3twCIAQlB3JxT41L+7FGgLSHlILUsLp+3uK/41lKDrgXByFN";
            //string keyPass = "123456";
            string key = "MIGHAgEAMBMGByqGSM49AgEGCCqBHM9VAYItBG0wawIBAQQgV3MGV0t9Bpuvx/qFOJC1Oh+hDrjvKiSdNXujYfwU+bChRANCAAQVTUc/lf8NfzuU+XKg+793v4wFWGjWYV+LL/+cO7qU6ZQ7q2iFvCBwQanOrz2TFHfPukZ8PNdXHlCbTv94F3Pz";
            string keyPass = "12345678abcd";
            //string key = "MIIBBjBhBgkqhkiG9w0BBQ0wVDA0BgkqhkiG9w0BBQwwJwQQDDliVgkRrM2zGLbz9aVbxAIDAQAAAgEQMAsGCSqBHM9VAYMRAjAcBggqgRzPVQFoAgQQ7v947KoKnL1fZYdL9F4xXASBoMvavPnz4Cx/VlajxhkzPByO3fBLDD6pE5PS+R558gs8AUPp36UnjGamH1COsuu6EeQ7HS4Y7cmEuH7XnbP9NV8vFTx0Qg/Vte2i/QSrQZj4ahDat1RZQkJg+1r4q/EpRbeSGvXyD7q9A6RDehbunKgDXGrx/6p4qoP/rZrD7GohjHaJLcQWftjYPVC1YFg8vNAY3n6rGsoobhDEZ6KbysU=";
            //string keyPass = "12345678abcd";
            using (var clinet = new HttpClient())
            {
                var sm2 = new Sm2();
                HttpRequestMessage request = new HttpRequestMessage();
                request.Method = HttpMethod.Get;
                var now = DateTime.Now;
                var permission = new Permission()
                {
                    User = "test",
                    Time = now,
                    Version = "sm2"
                };
                permission.Sign(sm2, key, keyPass);
                request.RequestUri = new Uri("http://scxiaoshou.i56yun.com");
                request.Headers.ProxyAuthorization = new System.Net.Http.Headers.AuthenticationHeaderValue(HeaderConsts.AuthorizationScheme, permission.ToString());
                var result = await clinet.SendAsync(request);
                while (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    if(result.Headers.ProxyAuthenticate != null)
                    {
                        var auth = result.Headers.ProxyAuthenticate.FirstOrDefault();
                        if(auth!=null)
                        {
                            DateTime srcNow; 
                            if(DateTime.TryParseExact(auth.Parameter,"yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out srcNow))
                            {
                                permission.Time = DateTime.UtcNow + (srcNow - now);
                                permission.Sign(sm2, key, keyPass);
                                request = new HttpRequestMessage();
                                request.Method = HttpMethod.Get;
                                request.RequestUri = new Uri("http://scxiaoshou.i56yun.com");
                                request.Headers.ProxyAuthorization = new System.Net.Http.Headers.AuthenticationHeaderValue(HeaderConsts.AuthorizationScheme, permission.ToString());
                                result = await clinet.SendAsync(request);
                            }
                        }
                    }
                }
                var rcnt = await result.Content.ReadAsStringAsync();
            }
        }
        [TestMethod]
        public async ValueTask TestRsaCall()
        {
            var pubkey = "MIIFTTCCAzWgAwIBAgIUEAOcdqhs+ZD+nQGJ8q5UmlwNlYswDQYJKoZIhvcNAQELBQAwTzELMAkGA1UEBhMCQ04xETAPBgNVBAgMCFNoYW5naGFpMREwDwYDVQQHDAhTaGFuZ2hhaTELMAkGA1UECgwCSVQxDTALBgNVBAMMBHNoeWwwHhcNMjUwMTIyMDkwMjUwWhcNMjYwMTIyMDkwMjUwWjBPMQswCQYDVQQGEwJDTjERMA8GA1UECAwIU2hhbmdoYWkxETAPBgNVBAcMCFNoYW5naGFpMQswCQYDVQQKDAJJVDENMAsGA1UEAwwEc2h5bDCCAiIwDQYJKoZIhvcNAQEBBQADggIPADCCAgoCggIBAKiRDUoNoDcJICrInBg9E6YCYdOa2sCKZGI/ueT9STSps6pAPD8MkLwqntrcuXRPB55ClnnAK+krHT5v2kfp5UJpcYZJllH1kgafLS49WNuMTuQeGyEOlMGIMeXSx3deT0suU+jmK0N/xByWdjRwE8j8AymxAE8gy+QRROmj9y1DHz7tNbZ4qzl6BuaUCdKi7ha2xiOYLN4KzASKMKQVC1fYKxfWWU1ii/9JSAue8ev6otWCgvL0f76xGEIxLdyQWbaAy5r0j9DXdTfErM+pTgLmWVq45sFrMr/d4xUaDWShuWQAPzVHHVQq1R/cuoJKffg6dpmM2YHlmklF4HZ/+cNIRQU5YNKCnJw1mc9nofvuOOq2YDOCJc/jRczU1J0unBHdI4NlBIk3zuDsXJFZh1cMXMbGT09mOMdVxM3fVER5aVcvatRMvqjl4CUnnMv5WrtGNUrF8wx8uI9F3Q1IRxgqSzbFeP93j72iMn4Usr03tbKT07NGLwBxfeTxSQg2JRoPBVYmVNu8OCRRQZ/bz2VUV68G5ztu8nUWzuNRtRGSmkEYAwi2EvGlQyr0wVtft6nFe9zuedijGNPYdZmC0WxavjscKPzfq0C1mmcyXpGT6NxcyhvKua8Tv/+lzHy1Nts+tT9E1eKTLF/bso6N/nclBHp5mKszM9T+gjN2gRC7AgMBAAGjITAfMB0GA1UdDgQWBBTCs9/xDJ07R9PFViTCas2L9Y0XRTANBgkqhkiG9w0BAQsFAAOCAgEACXxWFWP78rB95eUbtJLiThsfXrMyp3l0jgDx9IN7hdKM9aLS5mRz+0c4lVLkXgjoMJ72r/rfA4kgKsrynA3h5a0B9OPqNiGG9yp0sWgt50ZNZ8VGxdihViE+3O5D4TP2jts9Lwe/8aX6x5ptsI3wg8OYz+dyex9Ln3zKqK5ExQWyrAXwcZLMMmUumYk6l8Gb37jaZEwwDuxWVyP58c/8vjN+vBDsj6aOlwuFfxhGDD/QjXZ6VkhR9unhpJLS0lRdg54PETVod9wkRUHZs2mwN3m1XGS3rHj7yQzLAeQGq0YK0gpHXuiN9t1QRkXDqEODmJEI9ddJ94dtB4csopT0s+ytdrwwb/+XYWP0KRCtsZDoa9UEbGNg/GPh3BUC/4Vh0uR5XVf35WndhcaE1zgFYjK3faEINFstPe/NKl+eShgwfOXyUitOo8Pa795yYa/Cf+UgxT2aP+U7Vat0V/oqObGj2KJp5thnXQ5zkRV1kbSL+vEY9M9jTJYEl2kZZDSv1QzF2rblcUFikjmOHQ2IYnpfBeVI6R4r7RUoHzqFRmNrYlvZ9ZvJRrjtEKTsUXPcKadFHNQXVfz/MXfKXyaOt5Dj5hvfxnCigUFsvDlo43NrzioZqNd8wUTHfldI2ewgjPW9dpKzLTZPiakXNb8/hTFWLQE4U/Rmyf012zE5ooQ=";
            string key = "MIIWLwIBAzCCFeUGCSqGSIb3DQEHAaCCFdYEghXSMIIVzjCCC7oGCSqGSIb3DQEHBqCCC6swggunAgEAMIILoAYJKoZIhvcNAQcBMF8GCSqGSIb3DQEFDTBSMDEGCSqGSIb3DQEFDDAkBBBgLVZ63EHoHHKfYky/nTN4AgIIADAMBggqhkiG9w0CCQUAMB0GCWCGSAFlAwQBKgQQGNBg8sZO3+dE+sOU5EDKEICCCzDtmGTGC3ozYyR6Wsnj2pgOxwSBo5NqfIeowm848mLtuWgRhf4CVLqjJoFu2oJvZXY/GZ0UcbdRvy1QmZVzyAp2IPlj1i95ZgoNjzf1SF7XQszV/AseXVu4SpIzhYHX0FutRqg92vdUOBvUUico+hdtnCrLOs8YMChMISou6Y65EQe0ynSBYeqb8PRy4j5bylhFgMH5MdaP5YRwyWB5QvSitM7LYQAZizf/U1U4tEBYOHrRIOthddvOO8Pum2bq5klGW4Lpen5yU/82iBGZT8bSWhXOYRTa1yMjqq2gDhaTkwN+k/FyeBBiIE9VUoeIji3FtyA+1vYroNNHl4tGp7kKQ2mcTOTmTRIJfQzHrm1D4mfquyr/zw2oY6gUYGi+Lrd+B8URlm6hfe+9nsW0DR1rUylb046bxPW3+8G1K8ezTTLt6C9tP3xX25MnSgL1HpdjitB3wuFdLf+0XB0TkONrMaz4IXOZAs38fX+1RaHjngv0bwBMJmrWlg7nPsfFtAQcFgkQ4dbdGrMkJW8ufAA7O9i/h/iFP1IKPnIg6NxRxwQ6PqgdE6wrLHF/ca9n9Urj9ueCsDppfRmIefG1QDmMKw1lrHo9d6Vfm1VpGivvijkcH8W7ObyZdaRDoANYdChhn61eXoxNykrokF4Ij4CQkSnAFJJynm2Tp0AHZEmSG8nMX8RStvAc6/P1zTOojVONhBO5WVxF5GX7O7gKX1s0uBonKHQ7jNa/4vUxUyWrRMX2TvtA3hmvIApC+98oIsu5eYJw7Ys6haF/0xk2xJ490Pzm15qgmmGDY0PuH6VTZ7FStWiFHkp+cfJThMUbJkN46TOOJV9AZBUL9RyM1kY5zRBBi0uspETZWhHFAirXrjFxU4iDzClaLUeFQGsM8HpspOERShDvTaK1ZzEsRkjmy2D8xd3Xf2tHi6bT0rMtLSRjianUWm76XtJ1Tu/E5lljrxmHVVGxnRR88le6cTPmIA6SDcScNmFljS/HFq5cjReMi8XQ3+/X7S98hzHhRrAedCgWA3Bbh7W1hjFCxXK/nux9Jswq/+q+QAcyCtkxG0uY4/i2VpvrfKxpR5jrUe6Eiqts4Unvb1Pl7QM++bp3gWkO7Uwk+r2LFjEfQaMI+ShXWTyIZkqNyt2SF083PljW1D0INQHj6q7lBCb5e/GVuWKcp3/UILWmwooMAFT6vBe1tFTrMUu4kUcFyyyL8Pi327TNphVxHK267n8hJvYrzPwu5k2fschEzNTrBng/ZIea01SpMUIK5AZgT4qLrDMQJI5EYYAFbHzBzVtu8O/cJGV7msAVINmiyaSJ2zZBC3Hms3ixpYqXtEkT6ZjadwOKYR84qtHLuzM3yzHpOmwja3miYNJFLFHGh3kj2c3iS3WcpKCFxsim4gUGEtl3fx6FD714jLFVybIhNuVd2ORR+YcZNJojhXTZi+xpC7WNGXMXDUX5Mhx1uh8c0g/PQD2nk1y45CRxZbNwtmH2Han1LL1t4488MjX5dihxQlmPHcwI5GVO46US7Xqiv1GiqacVV+8r8JgY0WIy31i/jf7OKsqrRX0g6hIvQI8t85cmol6957+G3iM1UzSKeUFoF36F50Opcg5y+/wYbKbxI9HptQhqV/UF6MSKKLT1ANAA9w+cFw/PzwZEe/YSiTdTUQ8LlvH0ePzsMdczdSOyuoqJZDrFlmZWR4558n4GsgGmGtxFZNefkq9IDsCUWWwlycz5y+H10ZJHfJhrYFo0zWg3vAJ3Obvg+2jD9QCljgPohqxOvqyy0VRVhxe78PUHaTclkXfz6nfJ5IL1otbm7KMUivJpuhAbOGixnSyc/cG1mHEojyifRXRC8MghXHuwdt7gXTjaTxsMZu+cVk1PEFJ/GV/3NpCCT10JedUtILDXBVuhdPLhq8NuGIxS0e5i2v8Se9YxiKXeuwjhJNB+umyA5jSw6BYHY2TuDow6QxGDLBtJx/4B9MT+U3ADTqaDybJCXh7GB8RT+z0CpNDOBzNnlPJ0KCVJ6ht9f8moyVRB9km3PxsmBbSa36tO3o6V7eLLmsmvn4UjN6GyliXDuuyNI+rspnhqAqZxcZLrosjiNW4Zp8GdC0O9njx0Qxdcx9XBnHxDJdR03an67lojljYbRgFhFMNhYnBNq56bGmAxpywbWh6mobropIBm0xG0UrVjfoJNQfuHcobEmC0D7CY7gg6BGye5dZ0mgBibEzjnCrQx4fd13zQoEw3lcsME9vSqAaBe8cwRxH9kaSqoMZnwUKAzRcirv/zSlpEdkL7SAs2vqqlIusOjuqZxfWfH7njga7JtdQtfRHb8UGucraTHAQzIyM0CMtmIQSowzl00pTvIG/Tra2QozOTS+OebI3iaeUOWPDCeTtGhxDzTwlI91Qqoi6ytb7EA+hWCJHLa0y+8MdWtnVwvVr9kJgvI6pdR8XAsljR6/79GI3iJwJUOaQHWww+b0yVYeJq0KSfV0UdMiclPSZQjyRQNvfo27n9TLaIlBsvI7+nwAGdPl563yIejnsEb/m1W3dNND0/UyVhmGumm+VZIrY5RoPEYeabXNVf341henlkzvLa1LXo5f2xhW1dcFkneQ+hoN+vsEl74QMoyqdEGURb3InomAZ++zXJury8g/kyxIi8K4IJ8IoCkpBwTzUVSJsPCyLmjILcHlRI9+cPMjYNqeHx+h6RlA87qwP4iRJQk84BqVCYXbtjIpmCVa85IJgZsJit/OKVF7igFIETzWH3ryx6QoQ37TNAQOMyfKbRdJN+9n02pkeqZ0p1q38OjtXuxsQ8IsXkBcZ/62dGVlERr3HBVZ5ApVo5nbRdfcPN3SAJtIs9zBqrZghVfx2nlaQ2JN+hAFTU4IM+kmRPfT9l0VYEW0WYuqbXVhMxyMuCKfO7DC3sTSGK8CvSdxR0lPM/W7YBVv6A7Rt1BOEkCIY57YarzAEbzLcgOkAiqv9gxq1GOlyemOdTA1eQ3W9rz8AhAIYtV3dGKsLMPRbqnYonaAJDwLftadCaE8uruo01OFEePD1nAly4iHAduvNw7JYPDQeOFxdEyuWys19kB+7LvsTgSBQb6HbOCaNN+n6pvPRLIWozA7s/t88KLTLU66FRmyZBMR/EPhGLRDBhixZDKsz9geueEvA5R/I1Qy01XxKtvwaURejuwG1SVho0fXSE3Gp7QIhQH0Up9iF9MMzJL7R0FEM1+BjslPCVBXmJKXYEr8tPa1Uo30pr+BFZxh06VDnx1kOw7HPMao+Hp03CbvXE/URpH7tJwc52MJ9lnR+6nSusZjMQ06COLTjOCmzfSZJUZR6OPnLZ8TgxBGxlyni1DxWlIRRfCG5PdJVcadYMCeqXbR/BHluuj8uZLxmRAJ2bmJnxw7pi1+n1mYy6TIy1Uf/Shq302OIJpXiw0cGc4aZ/CzEgRXPg+BilTqTZwJI5ZYEuH0w6vi6aUade3pvlTR/9Jx5WnQXAQ/GmVZDkOmx58VqlsIpNQd2TKipBTvUnoACZN2Q4dbOz3olUCZK/4f/dK63hHmM2HHW18ikty5IjcDKQGL53SaAPljq09bDGqS9cm6HHYfxAlup+9a8EKDraX7OzIME7bGZx2ei+Ah1epXgcHlQV5836rzGFPdUue4CPRg5zY14qhL/j2ebw/55CO2lh/QgJKPfhDCS6nlrVNBjou/XUWilusOj03YXM6BxGHruh/Z/QiZO0FB1BhIB/sUy4wLA9u4pyPr05TZyVf7hQz7LC6FteBw6+aXN0Ygs2G5+uAosB7AFw3H0K61IGEtA8ZQUM4oitrUsZ7IpYLjzEon3MjtZDWLT75zjnEczCCCgwGCSqGSIb3DQEHAaCCCf0Eggn5MIIJ9TCCCfEGCyqGSIb3DQEMCgECoIIJuTCCCbUwXwYJKoZIhvcNAQUNMFIwMQYJKoZIhvcNAQUMMCQEENhup7bKNJOJLr5wJ5LVBI0CAggAMAwGCCqGSIb3DQIJBQAwHQYJYIZIAWUDBAEqBBArbSRJyhbMhFK3P5ZSyZ2vBIIJUIk0w8ikjWUBN67JrCguM0Ho1KxoGQp6I8IDmv39dRBHsrmfQHnyDD2utBj5gggUaGnc+gTiJsJeukTgLUyPy1v9abQO1zxjJevKOWevlV0VBJDBLQb9zA8VHibKkEKVzxdK6yl8OoSUcSE7UcaSlRMP2WLWC6Jjv5y8uVCquwjv0Vq/GJmGbQ1p9tnULwMhTfgC+yASrcSuVjclE6/SRdkOrhSxufgk+H2bideS65EzOtmKm67kGHnsEdd5uV0wi9mF3VEsNW7lLvByCDs6NbZykVSJiVA7ujt5zIu4eNxOfMCTnb8nVrFcrIusGeLNlMpA0gh/WednUv/FY4b+DdBMARAiV3v60IJL5nLqa2EqPl3DRrRg1FpZVbIyLJg13suVU+nbmeIExEaUbPhdqqALn5HDYAuJsZ2/TP4ClzB6DtSP3qW1NGjWji7JDbusFKZvoCxEPfF75ffNEBRn1SpcyKarIa1ewkyie5kJnhq8prBWFQjUKWv7WQSGSnxtss7zTeE0KPu+FEV27NdPuCUILI0BpTU3V/OvvX0ucRfHxFPTXkEMdL0KoehzT5HEqa5Cl+cDxAQWUU4tiJ8mcqAMV8vE785sAcZxvRrWI2ZxSAX1Ihn9F03T3+qCf2dnPWsVywsqnx8zSO5+lKhYTck181pNt6AkVSwLGt67zplUIHdG5w1t6TGNeCmco66Vfhf9ePB9oN/XVnQ1jKYWhN31LrUlRjje0gi+4Heg7hId6OsYrDuOyVRoR2fBuJMn1B0iGEZ1m9Owf0f/Bh82WPBcsGZ6oUKVUW75bc4X37a3JUALfX0J+ORQGJBjQxGavSDmrRiom9IhER4cviOvWhV1nT6hS7TncBjahar3cwMkPXnCKMm0YhQX9L30DFRD/1+1TyYfX8Aa8wARtID9vYYU1HZBanZmmfHFYTk8/zo8/1ohUAJkYdiGg8u/xog3f9xaXKEkuyPl2+czkDNkhdaguiswZGTNkPgyqPKrZN0TD8+kNLVrMVNZl/aHswmcmdP5GgdFCjPFt7jjGVTn1pI2krjn8YORhy3Aca5GkP4SmYYq99GyMieMo/lGMJnzqMgUs4OKoiD5yA01jefaPd+bphM2eMFc4saDYBKwMM+J6UEgVhNkIW3T3QDgglaiT/0p8Wzy7mTR7oY/0QKStjKYg4V9DHLwNyahuHbC/U2HRaPafqWv45ofT26kvVUPOujVKXvM9hvGKBm0Wv+wvdGKSwnJhplYiNE4A0om+eCReyNQHCjUSlhM7m6CpLbumvlQMfkUx3Vyb9PmpWGqqR31Rsc9qQ9cBu6mIVvabk3db7p2n5s9lL/mi/f0iCU/SXK92qGTXjWyT7DGJFrgXAqKG5WRI90wWOzYuo4uD50dWGy6Uj+idFbmbdNmijwsFR9tY1WNkmfd6rAiIVMyT6sewYshXjBjtMDu/MZvhsZQlfnYB2+qCefyKEFPwvzTMXYjThwTxrRTZs133LLFAuH2U8OLLz/q6LkECYntFdl9GMpayZbxw/hBWkDwkSstdRzLUlhjxMl0HIoxOcbdv7HhqR0lfgkUExiXdF1d4tOoAGj/nv1wbHJVhxbIO59ImAwlQvpbWczGO/hXKPGrnndMz8LnuEqv15Ky3AINOfGYRWgVfmQQkUOEQ/l8eFT8Wzls+J6ElNq3GMfks28G8Wmqo/52RqkFK8a3q13kVeSIhJyYzqp1XD1mljBvI1QSBbRPu3U/QkHfPXLh0bB05Ijf5VKKQ5oCtrk55f8AdAmWVk+pdGFkeoBmIjW6Ey2tgNfxvt+T5Sl6Nih+pDxq1DL5LyXy6Z8jAlQcE6KIrLOILspiJOn325r+dzL0xscmOc8q5nEMBYzKMFvAkKCtRdg0YZkcvk3Lj5skyX9R29EyObA15f4JUQHB54snV2UizB3JXHnGplsiPyLDTcWxObDgGjwDsaDx3nB/IiPYFJUUSsZ0jZ2aVeoicOQy+5lSniRoM8lPRL48Q3hyk4TG5/nw2ZbqB81fqbZswey5rv77dtn1slgayKdE4LI+Ldl834xHb5Tm4TH3iYuMs2ScHL1c9cGgg3jT3nnpRk+QS3bNrUxRrO6g2vRhHBjOF0RhnZD69UjPExzS37Gwk+A71qrnUG3d1aA8pJ1k18UT50zms7zyS/7Qz6dlra2NXs1AJpDK1O+NdQqz2HSto2rboXMXo8Sbkztft/KxdgfSAZHTQNk1lo5r3/tKG9yVEynC+AJaB6jKpT499bDePKYx844v+5tN7fntcXN8+nZv4wj0nq73Aah8wu3wKKGzp0usVurobOBUjR7xd44/AZYTSKvc2fdgt64Rk45h1OCFZ5uKGlTuoK01u6dF9YmSPggahjOzyVETzH4xb80EqBtly7VU3v/MiXAjFDkO6v9CDC7DMACrwA97+iMZRU+lgDI2kkdDJNKZi982hqJOC7F4I9esTT/BomlwTcpLn2eavZMrT1UxzoPseb4VQq0hgaOQWLIVM2vKT1kzmRytZB+fkcT93ehFYSIvUJW2USvr6vWyvlKZmORQwBnXqbNemCAKJ+MMIOgeq7+HnDQaOreTvMpxC/5XaFxdpmzdWuzOO1Pd3pF3aCRaF2J37y8DJ+Cdz15Qrzh70SB1lUmTYKIllKtTJ4w/CjEUCrscR2/Fi+adUPo6Frv8UOE8b6tFm5irFTk0cVLDU+FvYQ0PBaLf7+zl6djvvoU+d3BoktWYXZxDXWLkEW7ar8KEB8eupnopjpreG118GYWDi3cdAd6Yfe7lVB9rTJB/cAfIOJn8I9tP2g/BSUr/I+gRDJRnjUs06jQqiE/xvppOeA0KkvWH2it+v9jLd+vH3k9REZN4Cyvxsh5mQqBMbej8QRmwZ7tsSIoyBVD4tqGhATOWb+thKV6MzVROfbhXCrVg7/0VogpV2ZeyJI59EqSP21LJXyk8UwGxzkzZIV0/DzAR6Hk7NlcxGkgPA+aElYcx1b6tVmxsViYEaWCAALgkY5GAH8p6SM0Z56ObrcDnKjaX43WRM8d3v5oCq3FO0KxjzI2veRZPi7xB7e4oh1870ZT21YoSid3uoIvCgTVVVvCxc0dkJxD8jqCZKSn+YWrkQy7NTcuPNDIFxPywFZ8zY7FCw2iQYJ/h6kd2BQHmVXOaxiKB/BhJcqM5T0Qa0Rzf1qorP8s8MSUwIwYJKoZIhvcNAQkVMRYEFFESUFtRC3lffh4GXu6tik5BzxEbMEEwMTANBglghkgBZQMEAgEFAAQgPCAQIwScZe0t4O77khoXVo5zRe8cyuB6voYJNqjcbdEECDi/yb2FLbTXAgIIAA==";
            string keyPass = "12345678abcd";
            using (var clinet = new HttpClient())
            {
                var rsa = new Rsa();
                HttpRequestMessage request = new HttpRequestMessage();
                request.Method = HttpMethod.Get;
                var now = DateTime.Now;
                var permission = new Permission()
                {
                    User = "test",
                    Time = now,
                    Version = "rsa"
                };
                permission.Sign(rsa, key, keyPass);
                request.RequestUri = new Uri("http://scxiaoshou.i56yun.com");
                request.Headers.ProxyAuthorization = new System.Net.Http.Headers.AuthenticationHeaderValue(HeaderConsts.AuthorizationScheme, permission.ToString());
                var result = await clinet.SendAsync(request);
                while (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    if (result.Headers.ProxyAuthenticate != null)
                    {
                        var auth = result.Headers.ProxyAuthenticate.FirstOrDefault();
                        if (auth != null)
                        {
                            DateTime srcNow;
                            if (DateTime.TryParseExact(auth.Parameter, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out srcNow))
                            {
                                permission.Time = DateTime.UtcNow + (srcNow - now);
                                permission.Sign(rsa, key, keyPass);
                                request = new HttpRequestMessage();
                                request.Method = HttpMethod.Get;
                                request.RequestUri = new Uri("http://scxiaoshou.i56yun.com");
                                request.Headers.ProxyAuthorization = new System.Net.Http.Headers.AuthenticationHeaderValue(HeaderConsts.AuthorizationScheme, permission.ToString());
                                result = await clinet.SendAsync(request);
                            }
                        }
                    }
                }
                var rcnt = await result.Content.ReadAsStringAsync();
            }
        }
        [TestMethod]
        public async ValueTask TestSm2_PfxKeyCall()
        {
            var pubKey = "MIICADCCAaagAwIBAgIUQNlVrFK5BUhWsrnPNs/Y31g9X9IwCgYIKoEcz1UBg3UwXjELMAkGA1UEBhMCQ04xETAPBgNVBAgMCFNoYW5naGFpMREwDwYDVQQHDAhTaGFuZ2hhaTENMAsGA1UECgwEU0hZTDELMAkGA1UECwwCSVQxDTALBgNVBAMMBHNoemwwHhcNMjUwMTE2MDg1NTQ5WhcNMjYwMTE2MDg1NTQ5WjBeMQswCQYDVQQGEwJDTjERMA8GA1UECAwIU2hhbmdoYWkxETAPBgNVBAcMCFNoYW5naGFpMQ0wCwYDVQQKDARTSFlMMQswCQYDVQQLDAJJVDENMAsGA1UEAwwEc2h6bDBZMBMGByqGSM49AgEGCCqBHM9VAYItA0IABBVNRz+V/w1/O5T5cqD7v3e/jAVYaNZhX4sv/5w7upTplDuraIW8IHBBqc6vPZMUd8+6Rnw811ceUJtO/3gXc/OjQjBAMB0GA1UdDgQWBBSuMOYkR8P6SS7T3Nsin4VMgtrOgTAfBgNVHSMEGDAWgBTsme2nLFQyjA5yUxCsSX7fhx980DAKBggqgRzPVQGDdQNIADBFAiB0UajFdISz0NMVFfUFP0sREuRXtIhdyBFzeSWw2l81EwIhAPW1tYvKi/uf1S3txiaYXDw5ziTpH+t1CkhZef0r2kTj";
            string key = "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";
            string keyPass = "12345678abcd";
            using (var clinet = new HttpClient())
            {
                var sm2 = new Sm2(true);
                HttpRequestMessage request = new HttpRequestMessage();
                request.Method = HttpMethod.Get;
                var now = DateTime.Now;
                var permission = new Permission()
                {
                    User = "test",
                    Time = now,
                    Version = "sm2"
                };
                permission.Sign(sm2, key, keyPass);
                request.RequestUri = new Uri("http://scxiaoshou.i56yun.com");
                request.Headers.ProxyAuthorization = new System.Net.Http.Headers.AuthenticationHeaderValue(HeaderConsts.AuthorizationScheme, permission.ToString());
                var result = await clinet.SendAsync(request);
                while (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    if (result.Headers.ProxyAuthenticate != null)
                    {
                        var auth = result.Headers.ProxyAuthenticate.FirstOrDefault();
                        if (auth != null)
                        {
                            DateTime srcNow;
                            if (DateTime.TryParseExact(auth.Parameter, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out srcNow))
                            {
                                permission.Time = DateTime.UtcNow + (srcNow - now);
                                permission.Sign(sm2, key, keyPass);
                                request = new HttpRequestMessage();
                                request.Method = HttpMethod.Get;
                                request.RequestUri = new Uri("http://scxiaoshou.i56yun.com");
                                request.Headers.ProxyAuthorization = new System.Net.Http.Headers.AuthenticationHeaderValue(HeaderConsts.AuthorizationScheme, permission.ToString());
                                result = await clinet.SendAsync(request);
                            }
                        }
                    }
                }
                var rcnt = await result.Content.ReadAsStringAsync();
            }
        }
    }
}
