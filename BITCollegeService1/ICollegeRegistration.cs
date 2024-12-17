using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace BITCollegeService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICollegeRegistration" in both code and config file together.
    [ServiceContract]
    public interface ICollegeRegistration
    {
        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        void DoWork();

        /// <summary>
        /// Interface for DropCourse method
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        bool DropCourse(int registrationId);

        /// <summary>
        /// Interfae for RegisterCourse method
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        int RegisterCourse(int studentId, int courseId, String notes);

        /// <summary>
        /// Interface for UpdateGrade method
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        double? UpdateGrade(double grade, int registrationId, String notes);
    }
}
