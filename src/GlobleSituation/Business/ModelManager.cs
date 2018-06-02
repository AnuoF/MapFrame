
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlobleSituation.Model;
using GlobleSituation.Common;

namespace GlobleSituation.Business
{
    class ModelManager
    {
        private List<Model3D> models = new List<Model3D>();
        private int maxLength;

        public ModelManager(int maxLength = int.MaxValue)
        {
            this.maxLength = maxLength;
        }

        /// <summary>
        /// 是否已存在模型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool HasModel(Model3D model)
        {
            if (models.Find(o => o.ModelName == model.ModelName) != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 添加模型
        /// </summary>
        /// <param name="model">模型对象</param>
        public bool AddModel(Model3D model)
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
            Model3D m = models.Find(o => o.ModelName == modelName);
            if (m != null)
            {
                models.Remove(m);
            }
        }

        /// <summary>
        /// 根据条件查找模型
        /// </summary>
        /// <param name="match">条件</param>
        /// <returns></returns>
        public List<Model3D> FindModel(Predicate<Model3D> match)
        {
            return models.FindAll(match);
        }

        ///// <summary>
        ///// 根据索引查找模型
        ///// </summary>
        ///// <param name="match">条件</param>
        ///// <returns></returns>
        //public Model3D FindModel(int index)
        //{
        //    return models.Find(o => o.ModelIndex == index);
        //}

        /// <summary>
        /// 根据模型ID查找模型
        /// </summary>
        /// <param name="modelId"></param>
        /// <returns></returns>
        public Model3D FindModel(string modelId)
        {
            return models.Find(o => o.ModelName == modelId);
        }

        /// <summary>
        /// 刷新模型信息
        /// </summary>
        /// <param name="model"></param>
        public void UpdataModel(Model3D model)
        {
            int index = models.FindIndex(o => o.ModelName == model.ModelName);
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
