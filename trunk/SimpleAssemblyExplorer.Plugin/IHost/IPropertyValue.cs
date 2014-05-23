using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace SimpleAssemblyExplorer.Plugin
{
    public interface IPropertyValue
    {
        /// <summary>
        /// Add property to Settings
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="defaultValue"></param>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        bool AddProperty(string propertyName, object defaultValue, Type propertyType);

        /// <summary>
        /// Remove property from Settings
        /// </summary>
        /// <param name="propertyName"></param>
        void RemoveProperty(string propertyName);

        /// <summary>
        /// Set property value
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        void SetPropertyValue(string propertyName, object value);

        /// <summary>
        /// Get property value
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        object GetPropertyValue(string propertyName);

    }
}
