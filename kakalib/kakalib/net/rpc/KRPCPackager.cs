using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using KLib;

namespace KLib
{
    public class KRPCPackager : IRPCPackager
    {


        public Byte[] packRPCData(KRPCData rpcData)
        {

            var ba = new BinaryWriter(new MemoryStream());

            if (null == rpcData.request)
            {
                ba.Write((int)0);
            }
            else
            {
                var ba_request = packRequests(rpcData.request);
                ba.Write((int)ba_request.Length);
                ba.Write(ba_request, 0, ba_request.Length);
            }



            if (null == rpcData.response)
            {
                ba.Write((int)0);
            }
            else
            {
                var ba_response = packResponse(rpcData.response);
                ba.Write((int)ba_response.Length);
                ba.Write(ba_response, 0, ba_response.Length);
            }

            var bytes = new Byte[ba.BaseStream.Length];
            ba.BaseStream.Position = 0;
            ba.BaseStream.Read(bytes, 0, bytes.Length);
            
            return bytes;
        }

        public KRPCData unPackRPCData(Byte[] bytes)
        {

            var ba = new BinaryReader(new MemoryStream(bytes));

            var rpcData = new KRPCData();

            var len = ba.ReadInt32();

            if (len != 0)
            {

                var ba_request = ba.ReadBytes(len);
                rpcData.request = unPackRequests(ba_request);

            }

            len = ba.ReadInt32();
            if (len != 0)
            {

                var ba_response = ba.ReadBytes(len);
                rpcData.response = unPackResponse(ba_response);

            }

            return rpcData;

        }

        public Byte[] packRequests(KRPCBatchRequest batchRequest)
        {

            KDataPackager requestData = new KDataPackager();

            requestData.writeInt(batchRequest.requestId);

            KRPCRequest[] list_request = batchRequest.list_request;

            int i = 0;
            int len = list_request.Length;

            ///请求调用过程的数量
            requestData.writeInt(len);

            while (i < len)
            {

                KRPCRequest request = list_request[i];
                requestData.writeInt(request.procedureId);
                requestData.writeValue(request.vars);

                i++;
            }

            return requestData.data;

        }

        public KRPCBatchRequest unPackRequests(Byte[] bytes)
        {

            KDataPackager requestData = new KDataPackager(bytes);

            KRPCBatchRequest batchRequest = new KRPCBatchRequest();

            batchRequest.requestId = requestData.readInt();

            int i = 0;
            int len = requestData.readInt();

            KRPCRequest[] list_request = new KRPCRequest[len];

            while (i < len)
            {

                KRPCRequest request = new KRPCRequest();
                request.procedureId = requestData.readInt();
                request.vars = requestData.readValue();
                list_request[i] = request;

                i++;
            }

            batchRequest.list_request = list_request;

            return batchRequest;

        }

        public Byte[] packResponse(KRPCResponse response)
        {

            KDataPackager responseData = new KDataPackager();

            responseData.writeInt(response.requestId);
            responseData.writeValue(response.result);

            return responseData.data;

        }

        public KRPCResponse unPackResponse(Byte[] bytes)
        {
            KDataPackager responseData = new KDataPackager(bytes);

            KRPCResponse response = new KRPCResponse();

            response.requestId = responseData.readInt();

            response.result = responseData.readValue();

            return response;

        }

    }
}
