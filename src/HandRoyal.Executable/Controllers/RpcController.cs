#pragma warning disable S1481 // Unused local variables should be removed
using System.Text.Json;
using Libplanet.Action.State;
using Libplanet.Blockchain;
using Libplanet.Crypto;
using Libplanet.Node.Services;
using Libplanet.Types.Assets;
using Microsoft.AspNetCore.Mvc;

namespace HandRoyal.Executable.Controllers;

[Route("rpc")]
[ApiController]
public sealed class RpcController(
    IBlockChainService blockChainService,
    IWebHostEnvironment environment,
    ILogger<RpcController> logger)
    : ControllerBase
{
    private const string Version = "2.0";

    private readonly BlockChain _blockChain = blockChainService.BlockChain;

    private string ChainId => environment.IsDevelopment() ? "0xfffa8c" : "0xa8c";

    [HttpPost]
    public IActionResult HandleRpcRequest([FromBody] JsonElement request)
    {
        try
        {
            var method = GetMethod(request);
            return method switch
            {
                "eth_blockNumber" => OnBlockNumberRequest(request),
                "eth_getBlockByNumber" => OnGetBlockByNumberRequest(request),
                "eth_getBalance" => OnGetBalanceRequest(request),
                "net_version" => OnVersionRequest(request),
                "eth_gasPrice" => OnGasPriceRequest(request),
                "eth_getCode" => OnGetCodeRequest(request),
                "eth_chainId" => OnChainIdRequest(request),
                _ => OnDefaultRequest(request),
            };
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to handle RPC request: {0}", request);
            return BadRequest(new { jsonrpc = Version, error = e.Message });
        }
    }

    private static string GetMethod(JsonElement request)
    {
        if (request.TryGetProperty("method", out JsonElement methodElement) &&
            methodElement.ValueKind == JsonValueKind.String)
        {
            return methodElement.GetString()
             ?? throw new ArgumentException("Invalid request method");
        }

        throw new ArgumentException("Invalid request method");
    }

    private static object GetId(JsonElement request)
    {
        if (!request.TryGetProperty("id", out JsonElement idElement))
        {
            throw new ArgumentException("Invalid request ID");
        }

        if (idElement.ValueKind == JsonValueKind.Number)
        {
            return idElement.GetInt64();
        }

        if (idElement.ValueKind == JsonValueKind.String)
        {
            var idString = idElement.GetString()
                ?? throw new ArgumentException("Invalid request ID");
            if (long.TryParse(idString, out var id))
            {
                return id;
            }
            else if (Guid.TryParse(idString, out var guid))
            {
                return guid;
            }
        }

        throw new ArgumentException("Invalid request ID");
    }

    private OkObjectResult OnChainIdRequest(JsonElement request)
    {
        var id = GetId(request);
        return Ok(new
        {
            jsonrpc = Version,
            result = ChainId,
            id,
        });
    }

    private OkObjectResult OnGetCodeRequest(JsonElement request)
    {
        var id = GetId(request);
        var address = request.GetProperty("params")[0].GetString()
            ?? throw new ArgumentException("Invalid address");
        var height = request.GetProperty("params")[1].GetInt64();

        return new OkObjectResult(new
        {
            jsonrpc = Version,
            result = "0x",
            id,
        });
    }

    private OkObjectResult OnVersionRequest(JsonElement request)
    {
        return Ok(new
        {
            jsonrpc = Version,
            result = "1",
            id = GetId(request),
        });
    }

    private OkObjectResult OnGasPriceRequest(JsonElement request)
    {
        var id = GetId(request);
        var gasPrice = FungibleAssetValue.Parse(Currencies.Gas, "0.00001");
        return Ok(new
        {
            jsonrpc = Version,
            result = $"0x{gasPrice.RawValue:X}",
            id,
        });
    }

    private BadRequestObjectResult OnDefaultRequest(JsonElement request)
    {
        var id = GetId(request);
        return BadRequest(new
        {
            jsonrpc = Version,
            error = "Method not found",
            id,
        });
    }

    private OkObjectResult OnGetBalanceRequest(JsonElement request)
    {
        var id = GetId(request);
        var address = request.GetProperty("params")[0].GetString()
            ?? throw new ArgumentException("Invalid address");
        var height = request.GetProperty("params")[1].GetInt64();
        var block = _blockChain[height];
        var worldState = _blockChain.GetWorldState(block.Hash);
        var royal = worldState.GetBalance(new Address(address), Currencies.Royal);
        return Ok(new
        {
            jsonrpc = Version,
            result = $"0x{royal.RawValue:X}",
            id,
        });
    }

    private OkObjectResult OnGetBlockByNumberRequest(JsonElement request)
    {
        var id = GetId(request);
        var height = request.GetProperty("params")[0].GetInt32();
        var detailed = request.GetProperty("params")[1].GetBoolean();
        var block = _blockChain[height];
        if (detailed)
        {
            throw new InvalidOperationException("Detailed block retrieval is not supported yet.");
        }
        else
        {
            var response = new
            {
                jsonrpc = Version,
                result = block.Hash.ToString(),
                id,
            };
            return Ok(response);
        }
    }

    private OkObjectResult OnBlockNumberRequest(JsonElement request)
    {
        var id = GetId(request);
        var response = new
        {
            jsonrpc = Version,
            result = _blockChain.Tip.Index,
            id,
        };
        return Ok(response);
    }
}
