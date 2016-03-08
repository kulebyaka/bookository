using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookProgress.Web.Helpers
{
    public class PicklistField
    {
        public PicklistField(string label, int id)
        {
            Label = label;
            Id = id;
        }

        public PicklistField()
        {
        }

        public string Label { get; set; }

        public int? Id { get; set; }

        public override string ToString()
        {
            return Label;
        }
    }

    public class PicklistType<TEntity> 
    {
        private Func<TEntity, string> _labelGetFunc;

        private Func<TEntity, object> _valueGetFunc;

        private Func<TEntity, bool> _filterFunc;

        public PicklistType(Func<TEntity, string> labelGetFunc, Func<TEntity, object> valueGetFunc, Func<TEntity, bool> filterFunc = null)
        {
            _labelGetFunc = labelGetFunc;
            _valueGetFunc = valueGetFunc;
            _filterFunc = filterFunc;
        }

        public IEnumerable<SelectListItem> GetPickList()
        {
            return new List<SelectListItem>();
            //return GetAll(factory).Select(i => new SelectListItem() { Text = _labelGetFunc.Invoke(i), Value = _valueGetFunc.Invoke(i).ToString() });
        }

        //private List<TEntity> GetAll(BBServiceFactory factory)
        //{
        //    var propertyIsActive = typeof(TEntity).GetProperties().Where(p => p.IsIsActivePropertyOfBBEntity()).FirstOrDefault();
        //    // TODO: REMOVE && false after it will be fixed in backbone
        //    List<TEntity> values;
        //    if (propertyIsActive != null && false)
        //    {
        //        values = factory.GetByType<TEntity>().Search(propertyIsActive.Name + " == true").ResponseContent.List;
        //    }
        //    else
        //    {
        //        values = factory.GetByType<TEntity>().Search().ResponseContent.List;
        //    }

        //    if (_filterFunc != null)
        //    {
        //        values = values.Where(_filterFunc).ToList();
        //    }

        //    return values;

        //}

    }
}