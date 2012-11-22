using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wooha.Objects
{
    class CObjectCollection: CollectionBase
    {
        public CObjectCollection()
        {
        }

        public void Add(CGameObject o)
        {
            List.Add(o);
        }

        public void Remove(CGameObject o)
        {
            List.Remove(o);
        }

        public CGameObject Get(String guid)
        {
            foreach (CGameObject o in List)
            {
                if (o.ObjectId == guid)
                {
                    return o;
                }
                break;
            }
            return null;
        }

        public CGameObject this[int index]
        {
            get
            {
                return (CGameObject)List[index];
            }
            set
            {
                List[index] = value;
            }
        }

        public CGameObject this[String guid]
        {
            get
            {
                foreach (CGameObject o in List)
                {
                    if (o.ObjectId == guid)
                    {
                        return o;
                    }
                }
                return null;
            }
            set
            {
                foreach (CGameObject o in List)
                {
                    if (o.ObjectId == guid)
                    {
                        List.Remove(o);
                        List.Add(value);
                        break;
                    }
                }
            }
        }
    }
}
