﻿//------------------------------------------------------------------------------
// This is auto-generated code.
//------------------------------------------------------------------------------
// This code was generated by Entity Developer tool using NHibernate template.
// Generated by 1337
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace NHTest.Model
{

    /// <summary>
    /// There are no comments for NHTest.Model.Producer, NHTest in the schema.
    /// </summary>
    public partial class Producer : BaseEntity {
    
        #region Extensibility Method Definitions
        
        /// <summary>
        /// There are no comments for OnCreated in the schema.
        /// </summary>
        partial void OnCreated();

        public override bool Equals(object obj)
        {
          Producer toCompare = obj as Producer;
          if (toCompare == null)
          {
            return false;
          }

          if (!Object.Equals(this.Id, toCompare.Id))
            return false;
          
          return true;
        }

        public override int GetHashCode()
        {
          int hashCode = 13;
          hashCode = (hashCode * 7) + Id.GetHashCode();
          return hashCode;
        }
        
        #endregion
        /// <summary>
        /// There are no comments for Producer constructor in the schema.
        /// </summary>
        public Producer()
        {
            this.ProductLinkProducers = new HashSet<ProductLinkProducer>();
            OnCreated();
        }

    
        /// <summary>
        /// There are no comments for Id in the schema.
        /// </summary>
        public virtual long Id
        {
            get;
            set;
        }

    
        /// <summary>
        /// There are no comments for Name in the schema.
        /// </summary>
        public virtual string Name
        {
            get;
            set;
        }

    
        /// <summary>
        /// There are no comments for ProductLinkProducers in the schema.
        /// </summary>
        public virtual ISet<ProductLinkProducer> ProductLinkProducers
        {
            get;
            set;
        }
    }

}