using System;
using Skytable.Client;
using Skytable.Client.Querying;
using Xunit;

namespace Skytable.Client.IntegrationTests
{
    public class QueryTests : IClassFixture<SkytableFixture>
    {
        private SkytableFixture _fixture;

        public QueryTests(SkytableFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void DelQueryShouldReturnOne_WhenCalledWithExistingKey()
        {
            _fixture.Db.Set("DelKey", "DelValue");
            var result = _fixture.Db.Delete("DelKey");
            var delKeyCount = (ulong)result.Item.Object;
            Assert.True(result.IsOk);
            Assert.Equal((ulong)1, delKeyCount);
        }

        [Fact]
        public void DelQueryShouldReturnZero_WhenCalledWithMissingKey()
        {
            var result = _fixture.Db.Delete("MissingDeleteKey");
            var responseCode = result.Item.Object as ResponseCode;
            var delKeyCount = (ulong)result.Item.Object;
            Assert.True(result.IsOk);
            Assert.Equal((ulong)0, delKeyCount);
        }

        [Fact]
        public void SetQueryShouldReturnOk_WhenCalledWithNewKey()
        {
            var result = _fixture.Db.Set("SetKey", "SetValue");
            var responseCode = result.Item.Object as ResponseCode;
            Assert.True(result.IsOk);
            Assert.Equal(RespCode.Okay, responseCode.Code);
        }

        [Fact]
        public void GetQueryShouldReturnOk_WhenCalledWithExistingKey()
        {
            _fixture.Db.Set("GetKey", "GetValue");
            var result = _fixture.Db.Get("GetKey");
            Assert.True(result.IsOk);
            Assert.Equal("GetValue", result.Item.Object);
        }

        [Fact]
        public void GetQueryShouldReturnNotFound_WhenCalledWithMissingKey()
        {
            var result = _fixture.Db.Get("GetKeyMissing");
            var responseCode = result.Item.Object as ResponseCode;
            Assert.True(result.IsOk);
            Assert.Equal(RespCode.NotFound, responseCode.Code);
        }

        [Fact]
        public void USetQueryShouldReturnOk_WhenCalledWithNewKey()
        {
            var result = _fixture.Db.USet("USetKey", "USetValue");
            var setKeyCount = (ulong)result.Item.Object;
            Assert.True(result.IsOk);
            Assert.Equal((ulong)1, setKeyCount);

            result = _fixture.Db.Get("USetKey");
            Assert.True(result.IsOk);
            Assert.Equal("USetValue", result.Item.Object);
        }

        [Fact]
        public void USetQueryShouldReturnOk_WhenCalledWithExistingKey()
        {
            _fixture.Db.Set("USetKeyExisting", "USetValueExisting");
            var result = _fixture.Db.USet("USetKeyExisting", "USetValueExistingNew");
            var setKeyCount = (ulong)result.Item.Object;
            Assert.True(result.IsOk);
            Assert.Equal((ulong)1, setKeyCount);

            result = _fixture.Db.Get("USetKeyExisting");
            Assert.True(result.IsOk);
            Assert.Equal("USetValueExistingNew", result.Item.Object);
        }
    }
}
