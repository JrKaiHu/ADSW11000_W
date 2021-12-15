using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;

namespace SAWLib
{
    ///////////////////////////////////////////////////////////////// 
    // Designer for the Spindle control with support for a Dynamic Property Editor  
    // Must add reference to System.Design.dll 
    /////////////////////////////////////////////////////////////////
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    public class SpindleDesigner : ObjFormDesigner
    {
        //Declare a Shadow Property
        public bool UseIO
        {
            get
            {
                //get the value from the component
                if (this.Component != null && this.Component is Spindle)
                {
                    return (bool)this.ShadowProperties["UseIO"];
                }
                return false;
            }
            set
            {
                //set the value in the component
                if (this.Component != null && this.Component is Spindle)
                {
                    this.ShadowProperties["UseIO"] = value;
                }
                //Important - refresh the component's properties
                TypeDescriptor.Refresh(this.Component);
            }
        }

        protected override void PreFilterProperties(System.Collections.IDictionary properties)
        {
            base.PreFilterProperties(properties);

            //Shadow the UseIO property so we can show/hide related properties when it changes
            //this effectively copies the existing "UseIO" property and redirects gets/sets to the local implementation 
            PropertyDescriptor myEnabledTypeProp = TypeDescriptor.CreateProperty(typeof(SpindleDesigner),
                                                                                (PropertyDescriptor)properties["UseIO"],
                                                                                new Attribute[]
                                                                                { BrowsableAttribute.Yes, 
                                                                                new DefaultValueAttribute( false )
                                                                                });

            properties["UseIO"] = myEnabledTypeProp;

            if (UseIO == true)
            {
                HideProperty(properties, "COMPort");
                HideProperty(properties, "BaudRate");
                HideProperty(properties, "StopBits");
                HideProperty(properties, "DataBit");
                HideProperty(properties, "ParityCheck");
                UnhideProperty(properties, "SpindleRPM");
                UnhideProperty(properties, "OPEnable");
                UnhideProperty(properties, "SwitchOn");
            }
            if (UseIO == false)
            {
                UnhideProperty(properties, "COMPort");
                UnhideProperty(properties, "BaudRate");
                UnhideProperty(properties, "StopBits");
                UnhideProperty(properties, "DataBit");
                UnhideProperty(properties, "ParityCheck");
                HideProperty(properties, "SpindleRPM");
                HideProperty(properties, "OPEnable");
                HideProperty(properties, "SwitchOn");
            }
        }

        /// <summary>
        /// hides a property by making it not browsable 
        /// </summary>
        private void HideProperty(System.Collections.IDictionary properties, string propName)
        {
            PropertyDescriptor oldProp = properties[propName] as PropertyDescriptor;
            if (oldProp != null)
            {
                //copy everything but the browsable attribute
                List<Attribute> copiedAtts = CopyAttributes(oldProp, typeof(BrowsableAttribute));
                //add the attribute to make it not browsable
                copiedAtts.Add(BrowsableAttribute.No);
                //copy to an array 
                Attribute[] arr = new Attribute[copiedAtts.Count];
                copiedAtts.CopyTo(arr);

                PropertyDescriptor newProp = TypeDescriptor.CreateProperty(oldProp.ComponentType, oldProp, arr);
                properties[propName] = newProp;
            }
        }

        /// <summary>
        /// unhides a property by making it browsable 
        /// </summary>
        private void UnhideProperty(System.Collections.IDictionary properties, string propName)
        {
            PropertyDescriptor oldProp = properties[propName] as PropertyDescriptor;
            if (oldProp != null)
            {
                //copy everything but the browsable attribute
                List<Attribute> copiedAtts = CopyAttributes(oldProp, typeof(BrowsableAttribute));
                //add the attribute to make it browsable
                copiedAtts.Add(BrowsableAttribute.Yes);
                //copy to an array 
                Attribute[] arr = new Attribute[copiedAtts.Count];
                copiedAtts.CopyTo(arr);

                //create the new property
                PropertyDescriptor newProp = TypeDescriptor.CreateProperty(oldProp.ComponentType, oldProp, arr);
                properties[propName] = newProp;
            }
        }

        private List<Attribute> CopyAttributes(PropertyDescriptor property, params Type[] exclusions)
        {
            List<Attribute> ret = new List<Attribute>();
            List<Type> exclusionList = new List<Type>(exclusions);

            foreach (Attribute att in property.Attributes)
            {
                if (!exclusionList.Contains(att.GetType()))
                {
                    ret.Add(att);
                }
            }
            return ret;
        }
    }
}
