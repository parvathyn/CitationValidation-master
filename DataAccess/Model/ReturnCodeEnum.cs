﻿
namespace DataAccess.Model
{
    public enum ReturnCodeEnum : int
    {
        Error = -1,
        Success = 0,
        NoRecordExists = 1,
        MoreRecordsExists = 2,
        InvalidInputParameters = 3, 

        ///
        SqlError = 4

    }
}
