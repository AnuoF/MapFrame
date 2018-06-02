
using GlobleSituation.Common;
using GlobleSituation.Model;
using System;
using System.Collections.Generic;

namespace GlobleSituation.Business
{
    class PlaneManager
    {
        private List<Plane> models = new List<Plane>();
        private int maxLength;

        public PlaneManager(int maxLength = int.MaxValue)
        {
            this.maxLength = maxLength;
        }

        /// <summary>
        /// 是否已存在模型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool HasModel(Plane model)
        {
            if (models.Find(o => o.Name == model.Name) != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 添加模型
        /// </summary>
        /// <param name="model">模型对象</param>
        public bool AddModel(Plane model)
        {
            try
            {
                models.Add(model);
                return true;
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(ModelManager), ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 删除模型
        /// </summary>
        /// <param name="modelName">模型名字</param>
        public void DeleteModel(string modelName)
        {
            Plane p = models.Find(o => o.Name == modelName);

            if (p != null)
                models.Remove(p);
        }

        /// <summary>
        /// 根据条件查找模型
        /// </summary>
        /// <param name="match">条件</param>
        /// <returns></returns>
        public List<Plane> FindModel(Predicate<Plane> match)
        {
            return models.FindAll(match);
        }

        /// <summary>
        /// 根据模型ID查找模型
        /// </summary>
        /// <param name="modelId"></param>
        /// <returns></returns>
        public Plane FindModel(string modelId)
        {
            return models.Find(o => o.Name == modelId);
        }

        /// <summary>
        /// 刷新模型信息
        /// </summary>
        /// <param name="model"></param>
        public void UpdataModel(Plane model)
        {
            int index = models.FindIndex(o => o.Name == model.Name);
            if (index > -1)
            {
                models[index] = model;
            }
        }

        /// <summary>
        /// 清除数据
        /// </summary>
        public void ClearData()
        {
            models.Clear();
        }

    }
}
