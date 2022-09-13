using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;

namespace HMS.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class FromHeaderAttribute : ParameterBindingAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public override HttpParameterBinding GetBinding(HttpParameterDescriptor parameter)
        {
            var httpConfig = parameter.Configuration;
            var binder = new ModelBinderAttribute().GetModelBinder(httpConfig, parameter.ParameterType);

            return new ModelBinderParameterBinding(parameter, binder, new ValueProviderFactory[] {
                new HeaderValueProviderFactory()
            });
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class HeaderValueProviderFactory : ValueProviderFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        public override IValueProvider GetValueProvider(HttpActionContext actionContext)
        {
            return new HeaderValueProvide(actionContext.Request.Headers);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class HeaderValueProvide : IValueProvider
    {
        private readonly HttpRequestHeaders _headers;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="headers"></param>
        public HeaderValueProvide(HttpRequestHeaders headers)
        {
            _headers = headers;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public bool ContainsPrefix(string prefix)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ValueProviderResult GetValue(string key)
        {
            IEnumerable<string> values;
            if (key == "api_version")
            {
                if (_headers.TryGetValues(key, out values))
                {
                    var data = string.Join(",", values);

                    return new ValueProviderResult(values, data, CultureInfo.InvariantCulture);
                }
                else
                {
                    values = new string[] { "1.0" };
                    return new ValueProviderResult(values, string.Join(",", values), CultureInfo.InvariantCulture);
                }
            }
            else
            {
                if (!_headers.TryGetValues(key, out values))
                {

                    return null;
                }
                if (!values.Any(x => x == "null"))
                {
                    var data = string.Join(",", values);

                    return new ValueProviderResult(values, data, CultureInfo.InvariantCulture);
                }
                else
                {
                    values = new List<string>();
                    return new ValueProviderResult(values, null, CultureInfo.InvariantCulture);
                }
            }
        }
    }
}