using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Wooha.Objects;

namespace Wooha.InitData
{
    class ObjectsData: DictionaryBase
    {
        public ObjectsData()
        {
        }

        public void Add(String key, CGameObject value)
        {
            Dictionary.Add(key, value);
        }

        public void Remove(String key)
        {
            Dictionary.Remove(key);
        }

        public CGameObject this[String key]
        {
            get
            {
                return (CGameObject)Dictionary[key];
            }
            set
            {
                Dictionary[key] = value;
            }
        }
    }
}
