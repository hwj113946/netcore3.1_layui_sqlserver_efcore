using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace netcore
{
    public sealed class NullWithEmptyStringResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return type.GetProperties()
                       .Select(p =>
                       {
                           var jp = base.CreateProperty(p, memberSerialization);
                           jp.ValueProvider = new NullToEmptyStringValueProvider(p);
                           return jp;
                       }).ToList();
        }

        /// <summary>
        /// 将所有返回字段转换为小写
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        //protected override string ResolvePropertyName(string propertyName)
        //{
        //    return propertyName.ToLower();
        //}
    }

    public class NullToEmptyStringValueProvider : IValueProvider
    {
        PropertyInfo _MemberInfo;
        public NullToEmptyStringValueProvider(PropertyInfo memberInfo)
        {
            _MemberInfo = memberInfo;
        }

        public object GetValue(object target)
        {
            object result = _MemberInfo.GetValue(target);
            if (result == null)
            {
                var type = _MemberInfo.PropertyType;
                if (type == typeof(string)) result = "";
                //else if (type == typeof(DateTime?))
                //    result = new DateTime(1, 1, 1);
            }
            return result;
        }

        public void SetValue(object target, object value)
        {
            _MemberInfo.SetValue(target, value);
        }
    }
}
