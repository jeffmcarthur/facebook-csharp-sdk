﻿namespace Facebook
{
    using System;
    using System.IO;

    public partial class FacebookClient
    {
        /// <summary>
        /// Makes a request to the Facebook server.
        /// </summary>
        /// <param name="httpMethod">Http method. (GET/POST/DELETE)</param>
        /// <param name="path">The resource path or the resource url.</param>
        /// <param name="parameters">The parameters</param>
        /// <param name="resultType">The type of deserialize object into.</param>
        /// <returns>The json result with headers.</returns>
        public virtual object Api(string httpMethod, string path, object parameters, Type resultType)
        {
            Stream input;
            bool isLegacyRestApi;
            var httpHelper = PrepareRequest(httpMethod, path, parameters, resultType, out input, out isLegacyRestApi);

            if (input != null)
            {
                try
                {
                    using (var stream = httpHelper.OpenWrite())
                    {
                        // write input to requestStream
                        var buffer = new byte[BufferSize];
                        while (true)
                        {
                            int bytesRead = input.Read(buffer, 0, buffer.Length);
                            if (bytesRead <= 0) break;
                            stream.Write(buffer, 0, bytesRead);
                            stream.Flush();
                        }
                    }
                }
                catch (WebExceptionWrapper ex)
                {
                    if (ex.GetResponse() == null) throw;
                }
                finally
                {
                    input.Dispose();
                }
            }

            Stream responseStream = null;
            object result = null;
            try
            {
                responseStream = httpHelper.OpenRead();
            }
            catch (WebExceptionWrapper ex)
            {
                if (ex.GetResponse() == null) throw;
                responseStream = httpHelper.OpenRead();
            }
            finally
            {
                if (responseStream != null)
                {
                    string responseString;
                    using (var stream = responseStream)
                    {
                        var response = httpHelper.HttpWebResponse;
                        using (var reader = new StreamReader(stream))
                        {
                            responseString = reader.ReadToEnd();
                        }
                    }

                    result = ProcessResponse(httpHelper, responseString, resultType, isLegacyRestApi);
                }
            }

            return result;
        }

        public virtual object Get(string path)
        {
            throw new NotImplementedException();
        }

        public virtual object Get(object parameters)
        {
            throw new NotImplementedException();
        }

        public virtual object Get(string path, object parameters)
        {
            throw new NotImplementedException();
        }

        public virtual bool Post(object parameters)
        {
            throw new NotImplementedException();
        }

        public virtual object Post(string path, object parameters)
        {
            throw new NotImplementedException();
        }

        public virtual object Delete(string path)
        {
            throw new NotImplementedException();
        }
    }
}