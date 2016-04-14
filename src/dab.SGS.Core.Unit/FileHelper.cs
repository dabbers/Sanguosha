﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Json;
using Windows.Storage;
using Windows.Data.Json;

namespace dab.SGS.Core.Unit
{
    public class FileIOHelper
    {
        //Read the content from Json file 
        public static string ReadFromDefaultFile(string fileName)
        {
            string resp = String.Empty;

            try
            {
                /*   
                    Need to make sure using ConfigureAwait, GetAwaiter to avoid any file access errors 
                    Once the file is opened can use ReadTextAsync again with GetAwaiver as snow below  
                    Conver the text to JsonArray and then deserialize into the object of our own format using DataContractJsonSerializer as below 
                    For more details refer https://www.suchan.cz/2014/07/file-io-best-practices-in-windows-and-phone-apps-part-1-available-apis-and-file-exists-checking/ 
                */
                Uri appUri = new Uri(fileName);//File name should be prefixed with 'ms-appx:///Assets/* 
                StorageFile anjFile = StorageFile.GetFileFromApplicationUriAsync(appUri).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
                resp = FileIO.ReadTextAsync(anjFile).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
            }

            return resp;
        }
    }
}