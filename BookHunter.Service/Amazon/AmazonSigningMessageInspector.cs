using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text.RegularExpressions;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace BookHunter.Service.Amazon
{
    public class AmazonSigningMessageInspector : IClientMessageInspector
    {
        private readonly string _accessKeyId = "";
        private readonly string _secretKey = "";

        public AmazonSigningMessageInspector(string accessKeyId, string secretKey)
        {
            _accessKeyId = accessKeyId;
            _secretKey = secretKey;
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var operation = Regex.Match(request.Headers.Action, "[^/]+$").ToString();
            var now = DateTime.UtcNow;
            var timestamp = now.ToString("yyyy-MM-ddTHH:mm:ssZ");
            var signMe = operation + timestamp;

            var hmacsha256 = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithmNames.HmacSha256);
            var keyMaterial = CryptographicBuffer.ConvertStringToBinary(_secretKey, BinaryStringEncoding.Utf8);
            var hmacKey = hmacsha256.CreateKey(keyMaterial);
            var data = CryptographicBuffer.ConvertStringToBinary(signMe, BinaryStringEncoding.Utf8);
            var hash = CryptographicEngine.Sign(hmacKey, data);
            var signature = CryptographicBuffer.EncodeToBase64String(hash);

            // add the signature information to the request headers
            request.Headers.Add(new AmazonHeader("AWSAccessKeyId", _accessKeyId));
            request.Headers.Add(new AmazonHeader("Timestamp", timestamp));
            request.Headers.Add(new AmazonHeader("Signature", signature));

            return null;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
        }
    }
}