using System;
using System.Collections.Generic;
using System.Text;

namespace KLib
{
    public interface IRPCPackager
    {

        Byte[] packRPCData(KRPCData rpcData);

        KRPCData unPackRPCData(Byte[] ba);

        Byte[] packRequests(KRPCBatchRequest batchRequest);

        KRPCBatchRequest unPackRequests(Byte[] bytes);

        Byte[] packResponse(KRPCResponse response);

        KRPCResponse unPackResponse(Byte[] bytes);

    }
}
