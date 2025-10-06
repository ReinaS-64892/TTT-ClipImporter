Console.WriteLine("Write dependency version start");

Directory.SetCurrentDirectory("../"); // move to repository root
const string TEX_TRANS_CORE_PACKAGE_DOT_JSON_PATH = "../TexTransTool/package.json";
var texTransCorePackageJson = System.Text.Json.Nodes.JsonNode.Parse(File.ReadAllText(TEX_TRANS_CORE_PACKAGE_DOT_JSON_PATH));
if (texTransCorePackageJson is null) { throw new NullReferenceException(); }
var tttVersion = texTransCorePackageJson["version"]!.GetValue<string>();
var tttCode = texTransCorePackageJson["name"]!.GetValue<string>();


var tttPackageJsonPath = @"package.json";
var tttPackageJson = System.Text.Json.Nodes.JsonNode.Parse(File.ReadAllText(tttPackageJsonPath));
if (tttPackageJson is null) { throw new NullReferenceException(); }

tttPackageJson["dependencies"]![tttCode] = tttVersion;
tttPackageJson["vpmDependencies"]![tttCode] = "^" + tttVersion;


var outOpt = new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.General) { WriteIndented = true };
File.WriteAllText(tttPackageJsonPath, tttPackageJson.ToJsonString(outOpt) + "\n");
Console.WriteLine("Write version exit!");
