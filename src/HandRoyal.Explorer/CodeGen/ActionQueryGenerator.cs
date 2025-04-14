using System.Collections.Immutable;
using System.Reflection;
using System.Text;
using HandRoyal.Serialization;
using Libplanet.Action;
using Libplanet.Crypto;

#pragma warning disable MEN002 // Line is too long
namespace HandRoyal.Explorer.CodeGen
{
    public class ActionQueryGenerator
    {
        private readonly string _outputPath;
        private readonly string _namespace;

        public ActionQueryGenerator(string outputPath, string @namespace)
        {
            _outputPath = outputPath;
            _namespace = @namespace;
        }

        public void Generate()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var actionTypes = assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<ActionTypeAttribute>() != null)
                .ToList();

            // 디렉토리 생성
            Directory.CreateDirectory(Path.Combine(_outputPath, "Queries"));
            Directory.CreateDirectory(Path.Combine(_outputPath, "Mutations"));

            GenerateActionQueryController(actionTypes);
            GenerateActionController(actionTypes);
            GenerateMutationController(actionTypes);
        }

        private static void AddCommonUsings(StringBuilder sb)
        {
            sb.AppendLine("using System.Collections.Immutable;");
            sb.AppendLine("using System.Text;");
        }

        private static string ToCamelCase(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                 return str;
            }

            return char.ToLowerInvariant(str[0]) + str.Substring(1);
        }

        private static IEnumerable<PropertyInfo> GetActionProperties(Type actionType)
        {
            return actionType.GetProperties()
                .Select(p => (Property: p, Attribute: p.GetCustomAttribute<PropertyAttribute>()))
                .Where(x => x.Attribute != null)
                .OrderBy(x => x.Attribute!.Index)
                .Select(x => x.Property);
        }

        private void GenerateActionQueryController(IEnumerable<Type> actionTypes)
        {
            var sb = new StringBuilder();
            AddCommonUsings(sb);
            sb.AppendLine("using Bencodex;");
            sb.AppendLine("using GraphQL.AspNet.Attributes;");
            sb.AppendLine("using GraphQL.AspNet.Controllers;");
            sb.AppendLine("using HandRoyal.Actions;");
            sb.AppendLine("using HandRoyal.Explorer.Types;");
            sb.AppendLine("using Libplanet.Action;");
            sb.AppendLine("using Libplanet.Crypto;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_namespace}.Queries");
            sb.AppendLine("{");
            sb.AppendLine("    internal sealed class ActionQueryController : GraphController");
            sb.AppendLine("    {");
            sb.AppendLine("        private static readonly Codec _codec = new();");
            sb.AppendLine();

            foreach (var actionType in actionTypes)
            {
                GenerateActionMethod(sb, actionType, "Query", "HexValue", "Encode");
            }

            sb.AppendLine("        private static HexValue Encode(IAction action) => _codec.Encode(action.PlainValue);");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            File.WriteAllText(Path.Combine(_outputPath, "Queries", "ActionQueryController.cs"), sb.ToString());
        }

        private void GenerateActionController(IEnumerable<Type> actionTypes)
        {
            var sb = new StringBuilder();
            AddCommonUsings(sb);
            sb.AppendLine("using Bencodex;");
            sb.AppendLine("using GraphQL.AspNet.Attributes;");
            sb.AppendLine("using GraphQL.AspNet.Controllers;");
            sb.AppendLine("using HandRoyal.Actions;");
            sb.AppendLine("using HandRoyal.Explorer.Jwt;");
            sb.AppendLine("using HandRoyal.Wallet.Interfaces;");
            sb.AppendLine("using Libplanet.Action;");
            sb.AppendLine("using Libplanet.Crypto;");
            sb.AppendLine("using Libplanet.Node.Services;");
            sb.AppendLine("using Libplanet.Types.Tx;");
            sb.AppendLine("using Microsoft.AspNetCore.Http;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_namespace}.Mutations");
            sb.AppendLine("{");
            sb.AppendLine("    internal sealed class ActionController : GraphController");
            sb.AppendLine("    {");
            sb.AppendLine("        private readonly IBlockChainService _blockChainService;");
            sb.AppendLine("        private readonly IWalletService _walletService;");
            sb.AppendLine("        private readonly IHttpContextAccessor _httpContextAccessor;");
            sb.AppendLine("        private readonly JwtValidator _jwtValidator;");
            sb.AppendLine();
            sb.AppendLine("        public ActionController(");
            sb.AppendLine("            IBlockChainService blockChainService,");
            sb.AppendLine("            IWalletService walletService,");
            sb.AppendLine("            IHttpContextAccessor httpContextAccessor,");
            sb.AppendLine("            JwtValidator jwtValidator)");
            sb.AppendLine("        {");
            sb.AppendLine("            _blockChainService = blockChainService;");
            sb.AppendLine("            _walletService = walletService;");
            sb.AppendLine("            _httpContextAccessor = httpContextAccessor;");
            sb.AppendLine("            _jwtValidator = jwtValidator;");
            sb.AppendLine("        }");
            sb.AppendLine();

            foreach (var actionType in actionTypes)
            {
                GenerateWalletActionMethod(sb, actionType);
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            File.WriteAllText(Path.Combine(_outputPath, "Mutations", "ActionController.cs"), sb.ToString());
        }

        private void GenerateMutationController(IEnumerable<Type> actionTypes)
        {
            var sb = new StringBuilder();
            AddCommonUsings(sb);
            sb.AppendLine("using GraphQL.AspNet.Attributes;");
            sb.AppendLine("using GraphQL.AspNet.Controllers;");
            sb.AppendLine("using HandRoyal.Actions;");
            sb.AppendLine("using HandRoyal.Explorer.Types;");
            sb.AppendLine("using Libplanet.Crypto;");
            sb.AppendLine("using Libplanet.Node.Services;");
            sb.AppendLine("using Libplanet.Types.Tx;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_namespace}.Mutations");
            sb.AppendLine("{");
            sb.AppendLine("    internal sealed class MutationController : GraphController");
            sb.AppendLine("    {");
            sb.AppendLine("        private readonly IBlockChainService _blockChainService;");
            sb.AppendLine();
            sb.AppendLine("        public MutationController(IBlockChainService blockChainService)");
            sb.AppendLine("        {");
            sb.AppendLine("            _blockChainService = blockChainService;");
            sb.AppendLine("        }");
            sb.AppendLine();

            foreach (var actionType in actionTypes)
            {
                GeneratePrivateKeyActionMethod(sb, actionType);
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            File.WriteAllText(Path.Combine(_outputPath, "Mutations", "MutationController.cs"), sb.ToString());
        }

        private void GenerateActionMethod(StringBuilder sb, Type actionType, string attributeType, string returnType, string returnMethod)
        {
            var actionName = actionType.Name;
            var properties = GetActionProperties(actionType);

            // Method signature
            sb.AppendLine($"        [{attributeType}(\"{actionName}\")]");
            sb.Append($"        public {returnType} ");
            sb.Append(actionName);
            sb.Append("(");

            // Parameters
            var parameters = properties.Select(p => $"{GetCSharpType(p.PropertyType)} {ToCamelCase(p.Name)}");
            sb.Append(string.Join(", ", parameters));
            sb.AppendLine(")");
            sb.AppendLine("        {");
            sb.AppendLine($"            var {ToCamelCase(actionName)} = new {actionName}");
            sb.AppendLine("            {");

            // Property assignments
            sb.AppendLine(string.Join(Environment.NewLine, properties.Select(prop =>
                $"                {prop.Name} = {ToCamelCase(prop.Name)},")));

            sb.AppendLine("            };");
            sb.AppendLine($"            return {returnMethod}({ToCamelCase(actionName)});");
            sb.AppendLine("        }");
            sb.AppendLine();
        }

        private void GenerateWalletActionMethod(StringBuilder sb, Type actionType)
        {
            var actionName = actionType.Name;
            var properties = GetActionProperties(actionType);

            // Method signature
            sb.AppendLine($"        [MutationRoot(\"{actionName}ByWallet\")]");
            sb.Append("        public async Task<TxId> ");
            sb.Append(actionName);
            sb.Append("(");

            // Parameters
            var parameters = properties.Select(p => $"{GetCSharpType(p.PropertyType)} {ToCamelCase(p.Name)}");
            sb.Append(string.Join(", ", parameters));
            sb.AppendLine(")");
            sb.AppendLine("        {");
            sb.AppendLine($"            var {ToCamelCase(actionName)} = new {actionName}");
            sb.AppendLine("            {");

            // Property assignments
            sb.AppendLine(string.Join(Environment.NewLine, properties.Select(prop =>
                $"                {prop.Name} = {ToCamelCase(prop.Name)},")));

            sb.AppendLine("            };");
            sb.AppendLine();
            sb.AppendLine("            var userId = _httpContextAccessor.UserId();");
            sb.AppendLine("            var address = await _walletService.GetAddressAsync(userId);");
            sb.AppendLine("            var blockChain = _blockChainService.BlockChain;");
            sb.AppendLine("            var nonce = blockChain.GetNextTxNonce(address);");
            sb.AppendLine();
            sb.AppendLine("            var invoice = new TxInvoice(");
            sb.AppendLine("                genesisHash: blockChain.Genesis.Hash,");
            sb.AppendLine($"                actions: new TxActionList([{ToCamelCase(actionName)}.PlainValue]),");
            sb.AppendLine("                gasLimit: null,");
            sb.AppendLine("                maxGasPrice: null);");
            sb.AppendLine();
            sb.AppendLine("            var metaData = new TxSigningMetadata(address, nonce);");
            sb.AppendLine("            var unsignedTx = new UnsignedTx(invoice, metaData);");
            sb.AppendLine("            var payload = Encoding.UTF8.GetBytes(unsignedTx.SerializeUnsignedTx());");
            sb.AppendLine("            var signature = await _walletService.Sign(userId, payload);");
            sb.AppendLine("            var signedTx = new Transaction(unsignedTx, [..signature]);");
            sb.AppendLine();
            sb.AppendLine("            blockChain.StageTransaction(signedTx);");
            sb.AppendLine("            return signedTx.Id;");
            sb.AppendLine("        }");
            sb.AppendLine();
        }

        private void GeneratePrivateKeyActionMethod(StringBuilder sb, Type actionType)
        {
            var actionName = actionType.Name;
            var properties = GetActionProperties(actionType);

            // Method signature
            sb.AppendLine($"        [MutationRoot(\"{actionName}\")]");
            sb.Append("        public TxId ");
            sb.Append(actionName);
            sb.Append("(PrivateKey privateKey");

            // Parameters
            if (properties.Any())
            {
                sb.Append(", ");
                var parameters = properties.Select(p => $"{GetCSharpType(p.PropertyType)} {ToCamelCase(p.Name)}");
                sb.Append(string.Join(", ", parameters));
            }

            sb.AppendLine(")");
            sb.AppendLine("        {");
            sb.AppendLine($"            var {ToCamelCase(actionName)} = new {actionName}");
            sb.AppendLine("            {");

            // Property assignments
            sb.AppendLine(string.Join(Environment.NewLine, properties.Select(prop =>
                $"                {prop.Name} = {ToCamelCase(prop.Name)},")));

            sb.AppendLine("            };");
            sb.AppendLine();
            sb.AppendLine("            var txSettings = new TxSettings");
            sb.AppendLine("            {");
            sb.AppendLine("                PrivateKey = privateKey,");
            sb.AppendLine($"                Actions = [{ToCamelCase(actionName)}],");
            sb.AppendLine("            };");
            sb.AppendLine();
            sb.AppendLine("            return txSettings.StageTo(_blockChainService.BlockChain);");
            sb.AppendLine("        }");
            sb.AppendLine();
        }

        private string GetCSharpType(Type type)
        {
            if (type == typeof(string))
            {
                return "string";
            }

            if (type == typeof(int))
            {
                return "int";
            }

            if (type == typeof(long))
            {
                return "long";
            }

            if (type == typeof(bool))
            {
                return "bool";
            }

            if (type == typeof(Address))
            {
                return "Address";
            }

            if (type == typeof(ImmutableArray<Address>))
            {
                return "ImmutableArray<Address>";
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ImmutableArray<>))
            {
                var genericArg = type.GetGenericArguments()[0];
                return $"ImmutableArray<{GetCSharpType(genericArg)}>";
            }

            return type.Name;
        }
    }
}
#pragma warning restore MEN002 // Line is too long

