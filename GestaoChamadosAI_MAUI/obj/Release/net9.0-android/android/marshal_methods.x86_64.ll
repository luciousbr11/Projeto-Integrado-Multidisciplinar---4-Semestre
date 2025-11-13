; ModuleID = 'marshal_methods.x86_64.ll'
source_filename = "marshal_methods.x86_64.ll"
target datalayout = "e-m:e-p270:32:32-p271:32:32-p272:64:64-i64:64-f80:128-n8:16:32:64-S128"
target triple = "x86_64-unknown-linux-android21"

%struct.MarshalMethodName = type {
	i64, ; uint64_t id
	ptr ; char* name
}

%struct.MarshalMethodsManagedClass = type {
	i32, ; uint32_t token
	ptr ; MonoClass klass
}

@assembly_image_cache = dso_local local_unnamed_addr global [172 x ptr] zeroinitializer, align 16

; Each entry maps hash of an assembly name to an index into the `assembly_image_cache` array
@assembly_image_cache_hashes = dso_local local_unnamed_addr constant [516 x i64] [
	i64 u0x0071cf2d27b7d61e, ; 0: lib_Xamarin.AndroidX.SwipeRefreshLayout.dll.so => 88
	i64 u0x02123411c4e01926, ; 1: lib_Xamarin.AndroidX.Navigation.Runtime.dll.so => 83
	i64 u0x022e81ea9c46e03a, ; 2: lib_CommunityToolkit.Maui.Core.dll.so => 36
	i64 u0x02abedc11addc1ed, ; 3: lib_Mono.Android.Runtime.dll.so => 170
	i64 u0x032267b2a94db371, ; 4: lib_Xamarin.AndroidX.AppCompat.dll.so => 66
	i64 u0x043032f1d071fae0, ; 5: ru/Microsoft.Maui.Controls.resources => 24
	i64 u0x044440a55165631e, ; 6: lib-cs-Microsoft.Maui.Controls.resources.dll.so => 2
	i64 u0x046eb1581a80c6b0, ; 7: vi/Microsoft.Maui.Controls.resources => 30
	i64 u0x0517ef04e06e9f76, ; 8: System.Net.Primitives => 130
	i64 u0x0565d18c6da3de38, ; 9: Xamarin.AndroidX.RecyclerView => 85
	i64 u0x0581db89237110e9, ; 10: lib_System.Collections.dll.so => 103
	i64 u0x05989cb940b225a9, ; 11: Microsoft.Maui.dll => 60
	i64 u0x06076b5d2b581f08, ; 12: zh-HK/Microsoft.Maui.Controls.resources => 31
	i64 u0x06388ffe9f6c161a, ; 13: System.Xml.Linq.dll => 163
	i64 u0x0680a433c781bb3d, ; 14: Xamarin.AndroidX.Collection.Jvm => 69
	i64 u0x07c57877c7ba78ad, ; 15: ru/Microsoft.Maui.Controls.resources.dll => 24
	i64 u0x07dcdc7460a0c5e4, ; 16: System.Collections.NonGeneric => 101
	i64 u0x08a7c865576bbde7, ; 17: System.Reflection.Primitives => 143
	i64 u0x08f3c9788ee2153c, ; 18: Xamarin.AndroidX.DrawerLayout => 74
	i64 u0x090a04c5180cf016, ; 19: itext.styledxmlparser => 45
	i64 u0x0919c28b89381a0b, ; 20: lib_Microsoft.Extensions.Options.dll.so => 56
	i64 u0x092266563089ae3e, ; 21: lib_System.Collections.NonGeneric.dll.so => 101
	i64 u0x09d144a7e214d457, ; 22: System.Security.Cryptography => 153
	i64 u0x09e2b9f743db21a8, ; 23: lib_System.Reflection.Metadata.dll.so => 142
	i64 u0x0abb3e2b271edc45, ; 24: System.Threading.Channels.dll => 159
	i64 u0x0b3b632c3bbee20c, ; 25: sk/Microsoft.Maui.Controls.resources => 25
	i64 u0x0b6aff547b84fbe9, ; 26: Xamarin.KotlinX.Serialization.Core.Jvm => 95
	i64 u0x0be2e1f8ce4064ed, ; 27: Xamarin.AndroidX.ViewPager => 89
	i64 u0x0c3ca6cc978e2aae, ; 28: pt-BR/Microsoft.Maui.Controls.resources => 21
	i64 u0x0c59ad9fbbd43abe, ; 29: Mono.Android => 171
	i64 u0x0c7790f60165fc06, ; 30: lib_Microsoft.Maui.Essentials.dll.so => 61
	i64 u0x0cce4bce83380b7f, ; 31: Xamarin.AndroidX.Security.SecurityCrypto => 87
	i64 u0x0e14e73a54dda68e, ; 32: lib_System.Net.NameResolution.dll.so => 128
	i64 u0x102a31b45304b1da, ; 33: Xamarin.AndroidX.CustomView => 73
	i64 u0x10f6cfcbcf801616, ; 34: System.IO.Compression.Brotli => 116
	i64 u0x123639456fb056da, ; 35: System.Reflection.Emit.Lightweight.dll => 141
	i64 u0x125b7f94acb989db, ; 36: Xamarin.AndroidX.RecyclerView.dll => 85
	i64 u0x13a01de0cbc3f06c, ; 37: lib-fr-Microsoft.Maui.Controls.resources.dll.so => 8
	i64 u0x13f1e5e209e91af4, ; 38: lib_Java.Interop.dll.so => 169
	i64 u0x13f1e880c25d96d1, ; 39: he/Microsoft.Maui.Controls.resources => 9
	i64 u0x143d8ea60a6a4011, ; 40: Microsoft.Extensions.DependencyInjection.Abstractions => 52
	i64 u0x15bdc156ed462f2f, ; 41: lib_System.IO.FileSystem.dll.so => 119
	i64 u0x16054fdcb6b3098b, ; 42: Microsoft.Extensions.DependencyModel.dll => 53
	i64 u0x16bf2a22df043a09, ; 43: System.IO.Pipes.dll => 122
	i64 u0x16cd84ceca3b17f3, ; 44: lib_BouncyCastle.Crypto.dll.so => 64
	i64 u0x16eeae54c7ebcc08, ; 45: System.Reflection.dll => 144
	i64 u0x17125c9a85b4929f, ; 46: lib_netstandard.dll.so => 167
	i64 u0x17b56e25558a5d36, ; 47: lib-hu-Microsoft.Maui.Controls.resources.dll.so => 12
	i64 u0x17f9358913beb16a, ; 48: System.Text.Encodings.Web => 156
	i64 u0x18402a709e357f3b, ; 49: lib_Xamarin.KotlinX.Serialization.Core.Jvm.dll.so => 95
	i64 u0x18f0ce884e87d89a, ; 50: nb/Microsoft.Maui.Controls.resources.dll => 18
	i64 u0x1a91866a319e9259, ; 51: lib_System.Collections.Concurrent.dll.so => 99
	i64 u0x1aac34d1917ba5d3, ; 52: lib_System.dll.so => 166
	i64 u0x1aad60783ffa3e5b, ; 53: lib-th-Microsoft.Maui.Controls.resources.dll.so => 27
	i64 u0x1c753b5ff15bce1b, ; 54: Mono.Android.Runtime.dll => 170
	i64 u0x1da4110562816681, ; 55: Xamarin.AndroidX.Security.SecurityCrypto.dll => 87
	i64 u0x1e3d87657e9659bc, ; 56: Xamarin.AndroidX.Navigation.UI => 84
	i64 u0x1e71143913d56c10, ; 57: lib-ko-Microsoft.Maui.Controls.resources.dll.so => 16
	i64 u0x1ed8fcce5e9b50a0, ; 58: Microsoft.Extensions.Options.dll => 56
	i64 u0x209375905fcc1bad, ; 59: lib_System.IO.Compression.Brotli.dll.so => 116
	i64 u0x20fab3cf2dfbc8df, ; 60: lib_System.Diagnostics.Process.dll.so => 111
	i64 u0x2174319c0d835bc9, ; 61: System.Runtime => 152
	i64 u0x21cc7e445dcd5469, ; 62: System.Reflection.Emit.ILGeneration => 140
	i64 u0x220fd4f2e7c48170, ; 63: th/Microsoft.Maui.Controls.resources => 27
	i64 u0x224538d85ed15a82, ; 64: System.IO.Pipes => 122
	i64 u0x237be844f1f812c7, ; 65: System.Threading.Thread.dll => 160
	i64 u0x23986dd7e5d4fc01, ; 66: System.IO.FileSystem.Primitives.dll => 118
	i64 u0x2407aef2bbe8fadf, ; 67: System.Console => 107
	i64 u0x240abe014b27e7d3, ; 68: Xamarin.AndroidX.Core.dll => 71
	i64 u0x247619fe4413f8bf, ; 69: System.Runtime.Serialization.Primitives.dll => 151
	i64 u0x252073cc3caa62c2, ; 70: fr/Microsoft.Maui.Controls.resources.dll => 8
	i64 u0x256b8d41255f01b1, ; 71: Xamarin.Google.Crypto.Tink.Android => 92
	i64 u0x2662c629b96b0b30, ; 72: lib_Xamarin.Kotlin.StdLib.dll.so => 93
	i64 u0x268c1439f13bcc29, ; 73: lib_Microsoft.Extensions.Primitives.dll.so => 57
	i64 u0x26d077d9678fe34f, ; 74: System.IO.dll => 123
	i64 u0x273f3515de5faf0d, ; 75: id/Microsoft.Maui.Controls.resources.dll => 13
	i64 u0x2742545f9094896d, ; 76: hr/Microsoft.Maui.Controls.resources => 11
	i64 u0x27b2b16f3e9de038, ; 77: Xamarin.Google.Crypto.Tink.Android.dll => 92
	i64 u0x27b410442fad6cf1, ; 78: Java.Interop.dll => 169
	i64 u0x27b97e0d52c3034a, ; 79: System.Diagnostics.Debug => 109
	i64 u0x2801845a2c71fbfb, ; 80: System.Net.Primitives.dll => 130
	i64 u0x2a128783efe70ba0, ; 81: uk/Microsoft.Maui.Controls.resources.dll => 29
	i64 u0x2a3b095612184159, ; 82: lib_System.Net.NetworkInformation.dll.so => 129
	i64 u0x2a6507a5ffabdf28, ; 83: System.Diagnostics.TraceSource.dll => 112
	i64 u0x2a8556742ffd34ef, ; 84: itext.sign => 44
	i64 u0x2ad156c8e1354139, ; 85: fi/Microsoft.Maui.Controls.resources => 7
	i64 u0x2af298f63581d886, ; 86: System.Text.RegularExpressions.dll => 158
	i64 u0x2afc1c4f898552ee, ; 87: lib_System.Formats.Asn1.dll.so => 115
	i64 u0x2b148910ed40fbf9, ; 88: zh-Hant/Microsoft.Maui.Controls.resources.dll => 33
	i64 u0x2b56eeab97412d7a, ; 89: itext.pdfa.dll => 43
	i64 u0x2c8bd14bb93a7d82, ; 90: lib-pl-Microsoft.Maui.Controls.resources.dll.so => 20
	i64 u0x2cbd9262ca785540, ; 91: lib_System.Text.Encoding.CodePages.dll.so => 154
	i64 u0x2cc9e1fed6257257, ; 92: lib_System.Reflection.Emit.Lightweight.dll.so => 141
	i64 u0x2cd723e9fe623c7c, ; 93: lib_System.Private.Xml.Linq.dll.so => 138
	i64 u0x2d169d318a968379, ; 94: System.Threading.dll => 161
	i64 u0x2d47774b7d993f59, ; 95: sv/Microsoft.Maui.Controls.resources.dll => 26
	i64 u0x2db915caf23548d2, ; 96: System.Text.Json.dll => 157
	i64 u0x2e5a40c319acb800, ; 97: System.IO.FileSystem => 119
	i64 u0x2e6f1f226821322a, ; 98: el/Microsoft.Maui.Controls.resources.dll => 5
	i64 u0x2f02f94df3200fe5, ; 99: System.Diagnostics.Process => 111
	i64 u0x2f2e98e1c89b1aff, ; 100: System.Xml.ReaderWriter => 164
	i64 u0x309ee9eeec09a71e, ; 101: lib_Xamarin.AndroidX.Fragment.dll.so => 75
	i64 u0x31195fef5d8fb552, ; 102: _Microsoft.Android.Resource.Designer.dll => 34
	i64 u0x32243413e774362a, ; 103: Xamarin.AndroidX.CardView.dll => 68
	i64 u0x3235427f8d12dae1, ; 104: lib_System.Drawing.Primitives.dll.so => 113
	i64 u0x32524ae1e229f098, ; 105: itext.svg.dll => 46
	i64 u0x329753a17a517811, ; 106: fr/Microsoft.Maui.Controls.resources => 8
	i64 u0x32aa989ff07a84ff, ; 107: lib_System.Xml.ReaderWriter.dll.so => 164
	i64 u0x33829542f112d59b, ; 108: System.Collections.Immutable => 100
	i64 u0x33a31443733849fe, ; 109: lib-es-Microsoft.Maui.Controls.resources.dll.so => 6
	i64 u0x341abc357fbb4ebf, ; 110: lib_System.Net.Sockets.dll.so => 133
	i64 u0x34dfd74fe2afcf37, ; 111: Microsoft.Maui => 60
	i64 u0x34e292762d9615df, ; 112: cs/Microsoft.Maui.Controls.resources.dll => 2
	i64 u0x3508234247f48404, ; 113: Microsoft.Maui.Controls => 58
	i64 u0x3549870798b4cd30, ; 114: lib_Xamarin.AndroidX.ViewPager2.dll.so => 90
	i64 u0x355282fc1c909694, ; 115: Microsoft.Extensions.Configuration => 49
	i64 u0x3673b042508f5b6b, ; 116: lib_System.Runtime.Extensions.dll.so => 145
	i64 u0x36b2b50fdf589ae2, ; 117: System.Reflection.Emit.Lightweight => 141
	i64 u0x36cada77dc79928b, ; 118: System.IO.MemoryMappedFiles => 120
	i64 u0x374ef46b06791af6, ; 119: System.Reflection.Primitives.dll => 143
	i64 u0x37bc29f3183003b6, ; 120: lib_System.IO.dll.so => 123
	i64 u0x380134e03b1e160a, ; 121: System.Collections.Immutable.dll => 100
	i64 u0x385c17636bb6fe6e, ; 122: Xamarin.AndroidX.CustomView.dll => 73
	i64 u0x38869c811d74050e, ; 123: System.Net.NameResolution.dll => 128
	i64 u0x393c226616977fdb, ; 124: lib_Xamarin.AndroidX.ViewPager.dll.so => 89
	i64 u0x395e37c3334cf82a, ; 125: lib-ca-Microsoft.Maui.Controls.resources.dll.so => 1
	i64 u0x39aa39fda111d9d3, ; 126: Newtonsoft.Json => 63
	i64 u0x3a9ae914a83b6050, ; 127: itext.barcodes.dll => 38
	i64 u0x3b860f9932505633, ; 128: lib_System.Text.Encoding.Extensions.dll.so => 155
	i64 u0x3c7c495f58ac5ee9, ; 129: Xamarin.Kotlin.StdLib => 93
	i64 u0x3d46f0b995082740, ; 130: System.Xml.Linq => 163
	i64 u0x3d9c2a242b040a50, ; 131: lib_Xamarin.AndroidX.Core.dll.so => 71
	i64 u0x407a10bb4bf95829, ; 132: lib_Xamarin.AndroidX.Navigation.Common.dll.so => 81
	i64 u0x41cab042be111c34, ; 133: lib_Xamarin.AndroidX.AppCompat.AppCompatResources.dll.so => 67
	i64 u0x4202b91ac01ad789, ; 134: itext.barcodes => 38
	i64 u0x43375950ec7c1b6a, ; 135: netstandard.dll => 167
	i64 u0x434c4e1d9284cdae, ; 136: Mono.Android.dll => 171
	i64 u0x43950f84de7cc79a, ; 137: pl/Microsoft.Maui.Controls.resources.dll => 20
	i64 u0x448bd33429269b19, ; 138: Microsoft.CSharp => 97
	i64 u0x4499fa3c8e494654, ; 139: lib_System.Runtime.Serialization.Primitives.dll.so => 151
	i64 u0x4515080865a951a5, ; 140: Xamarin.Kotlin.StdLib.dll => 93
	i64 u0x45c40276a42e283e, ; 141: System.Diagnostics.TraceSource => 112
	i64 u0x45d443f2a29adc37, ; 142: System.AppContext.dll => 98
	i64 u0x45fcc9fd66f25095, ; 143: Microsoft.Extensions.DependencyModel => 53
	i64 u0x46a4213bc97fe5ae, ; 144: lib-ru-Microsoft.Maui.Controls.resources.dll.so => 24
	i64 u0x47358bd471172e1d, ; 145: lib_System.Xml.Linq.dll.so => 163
	i64 u0x47daf4e1afbada10, ; 146: pt/Microsoft.Maui.Controls.resources => 22
	i64 u0x480c0a47dd42dd81, ; 147: lib_System.IO.MemoryMappedFiles.dll.so => 120
	i64 u0x49e952f19a4e2022, ; 148: System.ObjectModel => 136
	i64 u0x4a5667b2462a664b, ; 149: lib_Xamarin.AndroidX.Navigation.UI.dll.so => 84
	i64 u0x4b07a0ed0ab33ff4, ; 150: System.Runtime.Extensions.dll => 145
	i64 u0x4b7b6532ded934b7, ; 151: System.Text.Json => 157
	i64 u0x4cc5f15266470798, ; 152: lib_Xamarin.AndroidX.Loader.dll.so => 80
	i64 u0x4cf6f67dc77aacd2, ; 153: System.Net.NetworkInformation.dll => 129
	i64 u0x4d479f968a05e504, ; 154: System.Linq.Expressions.dll => 124
	i64 u0x4d55a010ffc4faff, ; 155: System.Private.Xml => 139
	i64 u0x4d95fccc1f67c7ca, ; 156: System.Runtime.Loader.dll => 148
	i64 u0x4dcf44c3c9b076a2, ; 157: it/Microsoft.Maui.Controls.resources.dll => 14
	i64 u0x4dd9247f1d2c3235, ; 158: Xamarin.AndroidX.Loader.dll => 80
	i64 u0x4e32f00cb0937401, ; 159: Mono.Android.Runtime => 170
	i64 u0x4e5eea4668ac2b18, ; 160: System.Text.Encoding.CodePages => 154
	i64 u0x4e982534d67b56ba, ; 161: lib_itext.io.dll.so => 40
	i64 u0x4ebd0c4b82c5eefc, ; 162: lib_System.Threading.Channels.dll.so => 159
	i64 u0x4f21ee6ef9eb527e, ; 163: ca/Microsoft.Maui.Controls.resources => 1
	i64 u0x4fbc57e20df1874a, ; 164: itext.io.dll => 40
	i64 u0x5037f0be3c28c7a3, ; 165: lib_Microsoft.Maui.Controls.dll.so => 58
	i64 u0x5112ed116d87baf8, ; 166: CommunityToolkit.Mvvm => 37
	i64 u0x512c33621dd468cb, ; 167: lib_itext.kernel.dll.so => 41
	i64 u0x5131bbe80989093f, ; 168: Xamarin.AndroidX.Lifecycle.ViewModel.Android.dll => 78
	i64 u0x51bb8a2afe774e32, ; 169: System.Drawing => 114
	i64 u0x526ce79eb8e90527, ; 170: lib_System.Net.Primitives.dll.so => 130
	i64 u0x52829f00b4467c38, ; 171: lib_System.Data.Common.dll.so => 108
	i64 u0x529e5a460e733af4, ; 172: lib_itext.sign.dll.so => 44
	i64 u0x529ffe06f39ab8db, ; 173: Xamarin.AndroidX.Core => 71
	i64 u0x52ff996554dbf352, ; 174: Microsoft.Maui.Graphics => 62
	i64 u0x535f7e40e8fef8af, ; 175: lib-sk-Microsoft.Maui.Controls.resources.dll.so => 25
	i64 u0x53a96d5c86c9e194, ; 176: System.Net.NetworkInformation => 129
	i64 u0x53be1038a61e8d44, ; 177: System.Runtime.InteropServices.RuntimeInformation.dll => 146
	i64 u0x53c3014b9437e684, ; 178: lib-zh-HK-Microsoft.Maui.Controls.resources.dll.so => 31
	i64 u0x53d666fa678b6cea, ; 179: Microsoft.DotNet.PlatformAbstractions => 48
	i64 u0x54795225dd1587af, ; 180: lib_System.Runtime.dll.so => 152
	i64 u0x556e8b63b660ab8b, ; 181: Xamarin.AndroidX.Lifecycle.Common.Jvm.dll => 76
	i64 u0x5588627c9a108ec9, ; 182: System.Collections.Specialized => 102
	i64 u0x571c5cfbec5ae8e2, ; 183: System.Private.Uri => 137
	i64 u0x579a06fed6eec900, ; 184: System.Private.CoreLib.dll => 168
	i64 u0x57c542c14049b66d, ; 185: System.Diagnostics.DiagnosticSource => 110
	i64 u0x58601b2dda4a27b9, ; 186: lib-ja-Microsoft.Maui.Controls.resources.dll.so => 15
	i64 u0x58688d9af496b168, ; 187: Microsoft.Extensions.DependencyInjection.dll => 51
	i64 u0x595a356d23e8da9a, ; 188: lib_Microsoft.CSharp.dll.so => 97
	i64 u0x5a89a886ae30258d, ; 189: lib_Xamarin.AndroidX.CoordinatorLayout.dll.so => 70
	i64 u0x5a8f6699f4a1caa9, ; 190: lib_System.Threading.dll.so => 161
	i64 u0x5ae9cd33b15841bf, ; 191: System.ComponentModel => 106
	i64 u0x5b5f0e240a06a2a2, ; 192: da/Microsoft.Maui.Controls.resources.dll => 3
	i64 u0x5c393624b8176517, ; 193: lib_Microsoft.Extensions.Logging.dll.so => 54
	i64 u0x5d0a4a29b02d9d3c, ; 194: System.Net.WebHeaderCollection.dll => 134
	i64 u0x5db0cbbd1028510e, ; 195: lib_System.Runtime.InteropServices.dll.so => 147
	i64 u0x5db30905d3e5013b, ; 196: Xamarin.AndroidX.Collection.Jvm.dll => 69
	i64 u0x5e467bc8f09ad026, ; 197: System.Collections.Specialized.dll => 102
	i64 u0x5ea92fdb19ec8c4c, ; 198: System.Text.Encodings.Web.dll => 156
	i64 u0x5eb8046dd40e9ac3, ; 199: System.ComponentModel.Primitives => 104
	i64 u0x5f36ccf5c6a57e24, ; 200: System.Xml.ReaderWriter.dll => 164
	i64 u0x5f4294b9b63cb842, ; 201: System.Data.Common => 108
	i64 u0x5f9a2d823f664957, ; 202: lib-el-Microsoft.Maui.Controls.resources.dll.so => 5
	i64 u0x609f4b7b63d802d4, ; 203: lib_Microsoft.Extensions.DependencyInjection.dll.so => 51
	i64 u0x60cd4e33d7e60134, ; 204: Xamarin.KotlinX.Coroutines.Core.Jvm => 94
	i64 u0x60f62d786afcf130, ; 205: System.Memory => 126
	i64 u0x61bb78c89f867353, ; 206: System.IO => 123
	i64 u0x61be8d1299194243, ; 207: Microsoft.Maui.Controls.Xaml => 59
	i64 u0x61d2cba29557038f, ; 208: de/Microsoft.Maui.Controls.resources => 4
	i64 u0x61d88f399afb2f45, ; 209: lib_System.Runtime.Loader.dll.so => 148
	i64 u0x622eef6f9e59068d, ; 210: System.Private.CoreLib => 168
	i64 u0x637320c71840c561, ; 211: lib_itext.pdfa.dll.so => 43
	i64 u0x63f1f6883c1e23c2, ; 212: lib_System.Collections.Immutable.dll.so => 100
	i64 u0x6400f68068c1e9f1, ; 213: Xamarin.Google.Android.Material.dll => 91
	i64 u0x64587004560099b9, ; 214: System.Reflection => 144
	i64 u0x658f524e4aba7dad, ; 215: CommunityToolkit.Maui.dll => 35
	i64 u0x65ecac39144dd3cc, ; 216: Microsoft.Maui.Controls.dll => 58
	i64 u0x65ece51227bfa724, ; 217: lib_System.Runtime.Numerics.dll.so => 149
	i64 u0x6679b2337ee6b22a, ; 218: lib_System.IO.FileSystem.Primitives.dll.so => 118
	i64 u0x6692e924eade1b29, ; 219: lib_System.Console.dll.so => 107
	i64 u0x66a4e5c6a3fb0bae, ; 220: lib_Xamarin.AndroidX.Lifecycle.ViewModel.Android.dll.so => 78
	i64 u0x66d13304ce1a3efa, ; 221: Xamarin.AndroidX.CursorAdapter => 72
	i64 u0x68558ec653afa616, ; 222: lib-da-Microsoft.Maui.Controls.resources.dll.so => 3
	i64 u0x6872ec7a2e36b1ac, ; 223: System.Drawing.Primitives.dll => 113
	i64 u0x68fbbbe2eb455198, ; 224: System.Formats.Asn1 => 115
	i64 u0x69063fc0ba8e6bdd, ; 225: he/Microsoft.Maui.Controls.resources.dll => 9
	i64 u0x6907e2ec08eb5694, ; 226: lib_GestaoChamadosAI_MAUI.dll.so => 96
	i64 u0x6a4d7577b2317255, ; 227: System.Runtime.InteropServices.dll => 147
	i64 u0x6ace3b74b15ee4a4, ; 228: nb/Microsoft.Maui.Controls.resources => 18
	i64 u0x6d12bfaa99c72b1f, ; 229: lib_Microsoft.Maui.Graphics.dll.so => 62
	i64 u0x6d79993361e10ef2, ; 230: Microsoft.Extensions.Primitives => 57
	i64 u0x6d86d56b84c8eb71, ; 231: lib_Xamarin.AndroidX.CursorAdapter.dll.so => 72
	i64 u0x6d9bea6b3e895cf7, ; 232: Microsoft.Extensions.Primitives.dll => 57
	i64 u0x6e25a02c3833319a, ; 233: lib_Xamarin.AndroidX.Navigation.Fragment.dll.so => 82
	i64 u0x6ea9e4fb12d28eed, ; 234: GestaoChamadosAI_MAUI.dll => 96
	i64 u0x6fd2265da78b93a4, ; 235: lib_Microsoft.Maui.dll.so => 60
	i64 u0x6fdfc7de82c33008, ; 236: cs/Microsoft.Maui.Controls.resources => 2
	i64 u0x701cd46a1c25a5fe, ; 237: System.IO.FileSystem.dll => 119
	i64 u0x70e99f48c05cb921, ; 238: tr/Microsoft.Maui.Controls.resources.dll => 28
	i64 u0x70fd3deda22442d2, ; 239: lib-nb-Microsoft.Maui.Controls.resources.dll.so => 18
	i64 u0x71a495ea3761dde8, ; 240: lib-it-Microsoft.Maui.Controls.resources.dll.so => 14
	i64 u0x71ad672adbe48f35, ; 241: System.ComponentModel.Primitives.dll => 104
	i64 u0x72b1fb4109e08d7b, ; 242: lib-hr-Microsoft.Maui.Controls.resources.dll.so => 11
	i64 u0x73e4ce94e2eb6ffc, ; 243: lib_System.Memory.dll.so => 126
	i64 u0x755a91767330b3d4, ; 244: lib_Microsoft.Extensions.Configuration.dll.so => 49
	i64 u0x76012e7334db86e5, ; 245: lib_Xamarin.AndroidX.SavedState.dll.so => 86
	i64 u0x76ca07b878f44da0, ; 246: System.Runtime.Numerics.dll => 149
	i64 u0x780bc73597a503a9, ; 247: lib-ms-Microsoft.Maui.Controls.resources.dll.so => 17
	i64 u0x783606d1e53e7a1a, ; 248: th/Microsoft.Maui.Controls.resources.dll => 27
	i64 u0x78a45e51311409b6, ; 249: Xamarin.AndroidX.Fragment.dll => 75
	i64 u0x7a9a57d43b0845fa, ; 250: System.AppContext => 98
	i64 u0x7adb8da2ac89b647, ; 251: fi/Microsoft.Maui.Controls.resources.dll => 7
	i64 u0x7bef86a4335c4870, ; 252: System.ComponentModel.TypeConverter => 105
	i64 u0x7c0820144cd34d6a, ; 253: sk/Microsoft.Maui.Controls.resources.dll => 25
	i64 u0x7c2a0bd1e0f988fc, ; 254: lib-de-Microsoft.Maui.Controls.resources.dll.so => 4
	i64 u0x7cc637f941f716d0, ; 255: CommunityToolkit.Maui.Core => 36
	i64 u0x7d649b75d580bb42, ; 256: ms/Microsoft.Maui.Controls.resources.dll => 17
	i64 u0x7d8ee2bdc8e3aad1, ; 257: System.Numerics.Vectors => 135
	i64 u0x7dfc3d6d9d8d7b70, ; 258: System.Collections => 103
	i64 u0x7e946809d6008ef2, ; 259: lib_System.ObjectModel.dll.so => 136
	i64 u0x7ecc13347c8fd849, ; 260: lib_System.ComponentModel.dll.so => 106
	i64 u0x7f00ddd9b9ca5a13, ; 261: Xamarin.AndroidX.ViewPager.dll => 89
	i64 u0x7f9351cd44b1273f, ; 262: Microsoft.Extensions.Configuration.Abstractions => 50
	i64 u0x7fbd557c99b3ce6f, ; 263: lib_Xamarin.AndroidX.Lifecycle.LiveData.Core.dll.so => 77
	i64 u0x80da183a87731838, ; 264: System.Reflection.Metadata => 142
	i64 u0x812c069d5cdecc17, ; 265: System.dll => 166
	i64 u0x81ab745f6c0f5ce6, ; 266: zh-Hant/Microsoft.Maui.Controls.resources => 33
	i64 u0x8277f2be6b5ce05f, ; 267: Xamarin.AndroidX.AppCompat => 66
	i64 u0x828f06563b30bc50, ; 268: lib_Xamarin.AndroidX.CardView.dll.so => 68
	i64 u0x82df8f5532a10c59, ; 269: lib_System.Drawing.dll.so => 114
	i64 u0x82f6403342e12049, ; 270: uk/Microsoft.Maui.Controls.resources => 29
	i64 u0x833edc738697d898, ; 271: itext.layout.dll => 42
	i64 u0x8350268f9d350eec, ; 272: itext.commons => 47
	i64 u0x83c14ba66c8e2b8c, ; 273: zh-Hans/Microsoft.Maui.Controls.resources => 32
	i64 u0x84ae73148a4557d2, ; 274: lib_System.IO.Pipes.dll.so => 122
	i64 u0x86a909228dc7657b, ; 275: lib-zh-Hant-Microsoft.Maui.Controls.resources.dll.so => 33
	i64 u0x86b3e00c36b84509, ; 276: Microsoft.Extensions.Configuration.dll => 49
	i64 u0x87c69b87d9283884, ; 277: lib_System.Threading.Thread.dll.so => 160
	i64 u0x87f6569b25707834, ; 278: System.IO.Compression.Brotli.dll => 116
	i64 u0x8842b3a5d2d3fb36, ; 279: Microsoft.Maui.Essentials => 61
	i64 u0x88ba6bc4f7762b03, ; 280: lib_System.Reflection.dll.so => 144
	i64 u0x88bda98e0cffb7a9, ; 281: lib_Xamarin.KotlinX.Coroutines.Core.Jvm.dll.so => 94
	i64 u0x8930322c7bd8f768, ; 282: netstandard => 167
	i64 u0x897a606c9e39c75f, ; 283: lib_System.ComponentModel.Primitives.dll.so => 104
	i64 u0x89c5188089ec2cd5, ; 284: lib_System.Runtime.InteropServices.RuntimeInformation.dll.so => 146
	i64 u0x8ad229ea26432ee2, ; 285: Xamarin.AndroidX.Loader => 80
	i64 u0x8aed8bcfab24aa6d, ; 286: itext.svg => 46
	i64 u0x8b4ff5d0fdd5faa1, ; 287: lib_System.Diagnostics.DiagnosticSource.dll.so => 110
	i64 u0x8b8d01333a96d0b5, ; 288: System.Diagnostics.Process.dll => 111
	i64 u0x8b9ceca7acae3451, ; 289: lib-he-Microsoft.Maui.Controls.resources.dll.so => 9
	i64 u0x8d0f420977c2c1c7, ; 290: Xamarin.AndroidX.CursorAdapter.dll => 72
	i64 u0x8d7b8ab4b3310ead, ; 291: System.Threading => 161
	i64 u0x8da188285aadfe8e, ; 292: System.Collections.Concurrent => 99
	i64 u0x8ec6e06a61c1baeb, ; 293: lib_Newtonsoft.Json.dll.so => 63
	i64 u0x8ed807bfe9858dfc, ; 294: Xamarin.AndroidX.Navigation.Common => 81
	i64 u0x8ee08b8194a30f48, ; 295: lib-hi-Microsoft.Maui.Controls.resources.dll.so => 10
	i64 u0x8ef7601039857a44, ; 296: lib-ro-Microsoft.Maui.Controls.resources.dll.so => 23
	i64 u0x8f32c6f611f6ffab, ; 297: pt/Microsoft.Maui.Controls.resources.dll => 22
	i64 u0x8f8829d21c8985a4, ; 298: lib-pt-BR-Microsoft.Maui.Controls.resources.dll.so => 21
	i64 u0x90263f8448b8f572, ; 299: lib_System.Diagnostics.TraceSource.dll.so => 112
	i64 u0x903101b46fb73a04, ; 300: _Microsoft.Android.Resource.Designer => 34
	i64 u0x90393bd4865292f3, ; 301: lib_System.IO.Compression.dll.so => 117
	i64 u0x90634f86c5ebe2b5, ; 302: Xamarin.AndroidX.Lifecycle.ViewModel.Android => 78
	i64 u0x907b636704ad79ef, ; 303: lib_Microsoft.Maui.Controls.Xaml.dll.so => 59
	i64 u0x91418dc638b29e68, ; 304: lib_Xamarin.AndroidX.CustomView.dll.so => 73
	i64 u0x9157bd523cd7ed36, ; 305: lib_System.Text.Json.dll.so => 157
	i64 u0x91a74f07b30d37e2, ; 306: System.Linq.dll => 125
	i64 u0x91fa41a87223399f, ; 307: ca/Microsoft.Maui.Controls.resources.dll => 1
	i64 u0x93cfa73ab28d6e35, ; 308: ms/Microsoft.Maui.Controls.resources => 17
	i64 u0x944077d8ca3c6580, ; 309: System.IO.Compression.dll => 117
	i64 u0x967fc325e09bfa8c, ; 310: es/Microsoft.Maui.Controls.resources => 6
	i64 u0x9732d8dbddea3d9a, ; 311: id/Microsoft.Maui.Controls.resources => 13
	i64 u0x978be80e5210d31b, ; 312: Microsoft.Maui.Graphics.dll => 62
	i64 u0x97b8c771ea3e4220, ; 313: System.ComponentModel.dll => 106
	i64 u0x97e144c9d3c6976e, ; 314: System.Collections.Concurrent.dll => 99
	i64 u0x991d510397f92d9d, ; 315: System.Linq.Expressions => 124
	i64 u0x999cb19e1a04ffd3, ; 316: CommunityToolkit.Mvvm.dll => 37
	i64 u0x99a00ca5270c6878, ; 317: Xamarin.AndroidX.Navigation.Runtime => 83
	i64 u0x99cdc6d1f2d3a72f, ; 318: ko/Microsoft.Maui.Controls.resources.dll => 16
	i64 u0x9d5dbcf5a48583fe, ; 319: lib_Xamarin.AndroidX.Activity.dll.so => 65
	i64 u0x9d74dee1a7725f34, ; 320: Microsoft.Extensions.Configuration.Abstractions.dll => 50
	i64 u0x9e4534b6adaf6e84, ; 321: nl/Microsoft.Maui.Controls.resources => 19
	i64 u0x9e4b95dec42769f7, ; 322: System.Diagnostics.Debug.dll => 109
	i64 u0x9eaf1efdf6f7267e, ; 323: Xamarin.AndroidX.Navigation.Common.dll => 81
	i64 u0x9eeec7beda8fafa4, ; 324: BouncyCastle.Crypto.dll => 64
	i64 u0x9ef542cf1f78c506, ; 325: Xamarin.AndroidX.Lifecycle.LiveData.Core => 77
	i64 u0xa033e501b291e851, ; 326: itext.kernel => 41
	i64 u0xa0d8259f4cc284ec, ; 327: lib_System.Security.Cryptography.dll.so => 153
	i64 u0xa0e17ca50c77a225, ; 328: lib_Xamarin.Google.Crypto.Tink.Android.dll.so => 92
	i64 u0xa1440773ee9d341e, ; 329: Xamarin.Google.Android.Material => 91
	i64 u0xa1b9d7c27f47219f, ; 330: Xamarin.AndroidX.Navigation.UI.dll => 84
	i64 u0xa2572680829d2c7c, ; 331: System.IO.Pipelines.dll => 121
	i64 u0xa46aa1eaa214539b, ; 332: ko/Microsoft.Maui.Controls.resources => 16
	i64 u0xa4d20d2ff0563d26, ; 333: lib_CommunityToolkit.Mvvm.dll.so => 37
	i64 u0xa4edc8f2ceae241a, ; 334: System.Data.Common.dll => 108
	i64 u0xa5494f40f128ce6a, ; 335: System.Runtime.Serialization.Formatters.dll => 150
	i64 u0xa5e599d1e0524750, ; 336: System.Numerics.Vectors.dll => 135
	i64 u0xa5f1ba49b85dd355, ; 337: System.Security.Cryptography.dll => 153
	i64 u0xa60fdaa9af524b6a, ; 338: Microsoft.DotNet.PlatformAbstractions.dll => 48
	i64 u0xa67dbee13e1df9ca, ; 339: Xamarin.AndroidX.SavedState.dll => 86
	i64 u0xa684b098dd27b296, ; 340: lib_Xamarin.AndroidX.Security.SecurityCrypto.dll.so => 87
	i64 u0xa68a420042bb9b1f, ; 341: Xamarin.AndroidX.DrawerLayout.dll => 74
	i64 u0xa78ce3745383236a, ; 342: Xamarin.AndroidX.Lifecycle.Common.Jvm => 76
	i64 u0xa7c31b56b4dc7b33, ; 343: hu/Microsoft.Maui.Controls.resources => 12
	i64 u0xa964304b5631e28a, ; 344: CommunityToolkit.Maui.Core.dll => 36
	i64 u0xaa2219c8e3449ff5, ; 345: Microsoft.Extensions.Logging.Abstractions => 55
	i64 u0xaa443ac34067eeef, ; 346: System.Private.Xml.dll => 139
	i64 u0xaa52de307ef5d1dd, ; 347: System.Net.Http => 127
	i64 u0xaaaf86367285a918, ; 348: Microsoft.Extensions.DependencyInjection.Abstractions.dll => 52
	i64 u0xaaf84bb3f052a265, ; 349: el/Microsoft.Maui.Controls.resources => 5
	i64 u0xab9c1b2687d86b0b, ; 350: lib_System.Linq.Expressions.dll.so => 124
	i64 u0xac2af3fa195a15ce, ; 351: System.Runtime.Numerics => 149
	i64 u0xac5376a2a538dc10, ; 352: Xamarin.AndroidX.Lifecycle.LiveData.Core.dll => 77
	i64 u0xac98d31068e24591, ; 353: System.Xml.XDocument => 165
	i64 u0xacd46e002c3ccb97, ; 354: ro/Microsoft.Maui.Controls.resources => 23
	i64 u0xacf42eea7ef9cd12, ; 355: System.Threading.Channels => 159
	i64 u0xad89c07347f1bad6, ; 356: nl/Microsoft.Maui.Controls.resources.dll => 19
	i64 u0xadbb53caf78a79d2, ; 357: System.Web.HttpUtility => 162
	i64 u0xadc90ab061a9e6e4, ; 358: System.ComponentModel.TypeConverter.dll => 105
	i64 u0xadf511667bef3595, ; 359: System.Net.Security => 132
	i64 u0xae282bcd03739de7, ; 360: Java.Interop => 169
	i64 u0xae53579c90db1107, ; 361: System.ObjectModel.dll => 136
	i64 u0xaf2e760f9c91cb86, ; 362: itext.layout => 42
	i64 u0xafe29f45095518e7, ; 363: lib_Xamarin.AndroidX.Lifecycle.ViewModelSavedState.dll.so => 79
	i64 u0xb05cc42cd94c6d9d, ; 364: lib-sv-Microsoft.Maui.Controls.resources.dll.so => 26
	i64 u0xb220631954820169, ; 365: System.Text.RegularExpressions => 158
	i64 u0xb27d64a740cc8c9c, ; 366: lib_itext.styledxmlparser.dll.so => 45
	i64 u0xb2a3f67f3bf29fce, ; 367: da/Microsoft.Maui.Controls.resources => 3
	i64 u0xb3f0a0fcda8d3ebc, ; 368: Xamarin.AndroidX.CardView => 68
	i64 u0xb46be1aa6d4fff93, ; 369: hi/Microsoft.Maui.Controls.resources => 10
	i64 u0xb477491be13109d8, ; 370: ar/Microsoft.Maui.Controls.resources => 0
	i64 u0xb4bd7015ecee9d86, ; 371: System.IO.Pipelines => 121
	i64 u0xb4c8142c581fa7a2, ; 372: itext.forms.dll => 39
	i64 u0xb50d9ae4eea71e97, ; 373: lib_Microsoft.DotNet.PlatformAbstractions.dll.so => 48
	i64 u0xb5c7fcdafbc67ee4, ; 374: Microsoft.Extensions.Logging.Abstractions.dll => 55
	i64 u0xb71e58d502bd29dc, ; 375: itext.styledxmlparser.dll => 45
	i64 u0xb7212c4683a94afe, ; 376: System.Drawing.Primitives => 113
	i64 u0xb7b7753d1f319409, ; 377: sv/Microsoft.Maui.Controls.resources => 26
	i64 u0xb81a2c6e0aee50fe, ; 378: lib_System.Private.CoreLib.dll.so => 168
	i64 u0xb9185c33a1643eed, ; 379: Microsoft.CSharp.dll => 97
	i64 u0xb9f64d3b230def68, ; 380: lib-pt-Microsoft.Maui.Controls.resources.dll.so => 22
	i64 u0xb9fc3c8a556e3691, ; 381: ja/Microsoft.Maui.Controls.resources => 15
	i64 u0xba0f52acac7e7a84, ; 382: itext.kernel.dll => 41
	i64 u0xba4670aa94a2b3c6, ; 383: lib_System.Xml.XDocument.dll.so => 165
	i64 u0xba48785529705af9, ; 384: System.Collections.dll => 103
	i64 u0xbb65706fde942ce3, ; 385: System.Net.Sockets => 133
	i64 u0xbbd180354b67271a, ; 386: System.Runtime.Serialization.Formatters => 150
	i64 u0xbc41034a90e7d095, ; 387: lib_itext.forms.dll.so => 39
	i64 u0xbd0e2c0d55246576, ; 388: System.Net.Http.dll => 127
	i64 u0xbd437a2cdb333d0d, ; 389: Xamarin.AndroidX.ViewPager2 => 90
	i64 u0xbd7d91e34beaf455, ; 390: itext.sign.dll => 44
	i64 u0xbee38d4a88835966, ; 391: Xamarin.AndroidX.AppCompat.AppCompatResources => 67
	i64 u0xc040a4ab55817f58, ; 392: ar/Microsoft.Maui.Controls.resources.dll => 0
	i64 u0xc0d928351ab5ca77, ; 393: System.Console.dll => 107
	i64 u0xc12b8b3afa48329c, ; 394: lib_System.Linq.dll.so => 125
	i64 u0xc1ff9ae3cdb6e1e6, ; 395: Xamarin.AndroidX.Activity.dll => 65
	i64 u0xc28c50f32f81cc73, ; 396: ja/Microsoft.Maui.Controls.resources.dll => 15
	i64 u0xc2bcfec99f69365e, ; 397: Xamarin.AndroidX.ViewPager2.dll => 90
	i64 u0xc3492f8f90f96ce4, ; 398: lib_Microsoft.Extensions.DependencyModel.dll.so => 53
	i64 u0xc4d3858ed4d08512, ; 399: Xamarin.AndroidX.Lifecycle.ViewModelSavedState.dll => 79
	i64 u0xc50fded0ded1418c, ; 400: lib_System.ComponentModel.TypeConverter.dll.so => 105
	i64 u0xc519125d6bc8fb11, ; 401: lib_System.Net.Requests.dll.so => 131
	i64 u0xc5293b19e4dc230e, ; 402: Xamarin.AndroidX.Navigation.Fragment => 82
	i64 u0xc5325b2fcb37446f, ; 403: lib_System.Private.Xml.dll.so => 139
	i64 u0xc5a0f4b95a699af7, ; 404: lib_System.Private.Uri.dll.so => 137
	i64 u0xc748e0d3daa1d20a, ; 405: GestaoChamadosAI_MAUI => 96
	i64 u0xc7c01e7d7c93a110, ; 406: System.Text.Encoding.Extensions.dll => 155
	i64 u0xc7ce851898a4548e, ; 407: lib_System.Web.HttpUtility.dll.so => 162
	i64 u0xc858a28d9ee5a6c5, ; 408: lib_System.Collections.Specialized.dll.so => 102
	i64 u0xc9e54b32fc19baf3, ; 409: lib_CommunityToolkit.Maui.dll.so => 35
	i64 u0xca3a723e7342c5b6, ; 410: lib-tr-Microsoft.Maui.Controls.resources.dll.so => 28
	i64 u0xcab3493c70141c2d, ; 411: pl/Microsoft.Maui.Controls.resources => 20
	i64 u0xcacfddc9f7c6de76, ; 412: ro/Microsoft.Maui.Controls.resources.dll => 23
	i64 u0xcbd4fdd9cef4a294, ; 413: lib__Microsoft.Android.Resource.Designer.dll.so => 34
	i64 u0xcc2876b32ef2794c, ; 414: lib_System.Text.RegularExpressions.dll.so => 158
	i64 u0xcc5c3bb714c4561e, ; 415: Xamarin.KotlinX.Coroutines.Core.Jvm.dll => 94
	i64 u0xcc76886e09b88260, ; 416: Xamarin.KotlinX.Serialization.Core.Jvm.dll => 95
	i64 u0xccf25c4b634ccd3a, ; 417: zh-Hans/Microsoft.Maui.Controls.resources.dll => 32
	i64 u0xcd10a42808629144, ; 418: System.Net.Requests => 131
	i64 u0xcd235365bb1cf97f, ; 419: lib_itext.svg.dll.so => 46
	i64 u0xcdd0c48b6937b21c, ; 420: Xamarin.AndroidX.SwipeRefreshLayout => 88
	i64 u0xcf23d8093f3ceadf, ; 421: System.Diagnostics.DiagnosticSource.dll => 110
	i64 u0xcf8fc898f98b0d34, ; 422: System.Private.Xml.Linq => 138
	i64 u0xd04b5f59ed596e31, ; 423: System.Reflection.Metadata.dll => 142
	i64 u0xd0af5414344dd23a, ; 424: itext.io => 40
	i64 u0xd0fc33d5ae5d4cb8, ; 425: System.Runtime.Extensions => 145
	i64 u0xd1194e1d8a8de83c, ; 426: lib_Xamarin.AndroidX.Lifecycle.Common.Jvm.dll.so => 76
	i64 u0xd1dcf65a5c5b2e92, ; 427: itext.pdfa => 43
	i64 u0xd333d0af9e423810, ; 428: System.Runtime.InteropServices => 147
	i64 u0xd3426d966bb704f5, ; 429: Xamarin.AndroidX.AppCompat.AppCompatResources.dll => 67
	i64 u0xd3651b6fc3125825, ; 430: System.Private.Uri.dll => 137
	i64 u0xd373685349b1fe8b, ; 431: Microsoft.Extensions.Logging.dll => 54
	i64 u0xd3e4c8d6a2d5d470, ; 432: it/Microsoft.Maui.Controls.resources => 14
	i64 u0xd4645626dffec99d, ; 433: lib_Microsoft.Extensions.DependencyInjection.Abstractions.dll.so => 52
	i64 u0xd5507e11a2b2839f, ; 434: Xamarin.AndroidX.Lifecycle.ViewModelSavedState => 79
	i64 u0xd6694f8359737e4e, ; 435: Xamarin.AndroidX.SavedState => 86
	i64 u0xd6d21782156bc35b, ; 436: Xamarin.AndroidX.SwipeRefreshLayout.dll => 88
	i64 u0xd6f8b724c5400fc8, ; 437: BouncyCastle.Crypto => 64
	i64 u0xd72329819cbbbc44, ; 438: lib_Microsoft.Extensions.Configuration.Abstractions.dll.so => 50
	i64 u0xd7b3764ada9d341d, ; 439: lib_Microsoft.Extensions.Logging.Abstractions.dll.so => 55
	i64 u0xd9fc7e791253de8f, ; 440: lib_itext.commons.dll.so => 47
	i64 u0xda1dfa4c534a9251, ; 441: Microsoft.Extensions.DependencyInjection => 51
	i64 u0xdad05a11827959a3, ; 442: System.Collections.NonGeneric.dll => 101
	i64 u0xdaefdfe71aa53cf9, ; 443: System.IO.FileSystem.Primitives => 118
	i64 u0xdb5383ab5865c007, ; 444: lib-vi-Microsoft.Maui.Controls.resources.dll.so => 30
	i64 u0xdb58816721c02a59, ; 445: lib_System.Reflection.Emit.ILGeneration.dll.so => 140
	i64 u0xdbeda89f832aa805, ; 446: vi/Microsoft.Maui.Controls.resources.dll => 30
	i64 u0xdbf9607a441b4505, ; 447: System.Linq => 125
	i64 u0xdce2c53525640bf3, ; 448: Microsoft.Extensions.Logging => 54
	i64 u0xdd2b722d78ef5f43, ; 449: System.Runtime.dll => 152
	i64 u0xdd67031857c72f96, ; 450: lib_System.Text.Encodings.Web.dll.so => 156
	i64 u0xdde30e6b77aa6f6c, ; 451: lib-zh-Hans-Microsoft.Maui.Controls.resources.dll.so => 32
	i64 u0xde110ae80fa7c2e2, ; 452: System.Xml.XDocument.dll => 165
	i64 u0xde8769ebda7d8647, ; 453: hr/Microsoft.Maui.Controls.resources.dll => 11
	i64 u0xe0142572c095a480, ; 454: Xamarin.AndroidX.AppCompat.dll => 66
	i64 u0xe02f89350ec78051, ; 455: Xamarin.AndroidX.CoordinatorLayout.dll => 70
	i64 u0xe192a588d4410686, ; 456: lib_System.IO.Pipelines.dll.so => 121
	i64 u0xe1a08bd3fa539e0d, ; 457: System.Runtime.Loader => 148
	i64 u0xe1b52f9f816c70ef, ; 458: System.Private.Xml.Linq.dll => 138
	i64 u0xe1ecfdb7fff86067, ; 459: System.Net.Security.dll => 132
	i64 u0xe2420585aeceb728, ; 460: System.Net.Requests.dll => 131
	i64 u0xe29b73bc11392966, ; 461: lib-id-Microsoft.Maui.Controls.resources.dll.so => 13
	i64 u0xe3811d68d4fe8463, ; 462: pt-BR/Microsoft.Maui.Controls.resources.dll => 21
	i64 u0xe494f7ced4ecd10a, ; 463: hu/Microsoft.Maui.Controls.resources.dll => 12
	i64 u0xe4a9b1e40d1e8917, ; 464: lib-fi-Microsoft.Maui.Controls.resources.dll.so => 7
	i64 u0xe4f74a0b5bf9703f, ; 465: System.Runtime.Serialization.Primitives => 151
	i64 u0xe5434e8a119ceb69, ; 466: lib_Mono.Android.dll.so => 171
	i64 u0xe8159f0f339a522f, ; 467: lib_itext.barcodes.dll.so => 38
	i64 u0xe89a2a9ef110899b, ; 468: System.Drawing.dll => 114
	i64 u0xedc4817167106c23, ; 469: System.Net.Sockets.dll => 133
	i64 u0xedc632067fb20ff3, ; 470: System.Memory.dll => 126
	i64 u0xedc8e4ca71a02a8b, ; 471: Xamarin.AndroidX.Navigation.Runtime.dll => 83
	i64 u0xeeb7ebb80150501b, ; 472: lib_Xamarin.AndroidX.Collection.Jvm.dll.so => 69
	i64 u0xef03b1b5a04e9709, ; 473: System.Text.Encoding.CodePages.dll => 154
	i64 u0xef6e6d3ed7611955, ; 474: itext.forms => 39
	i64 u0xef72742e1bcca27a, ; 475: Microsoft.Maui.Essentials.dll => 61
	i64 u0xefec0b7fdc57ec42, ; 476: Xamarin.AndroidX.Activity => 65
	i64 u0xf00c29406ea45e19, ; 477: es/Microsoft.Maui.Controls.resources.dll => 6
	i64 u0xf09e47b6ae914f6e, ; 478: System.Net.NameResolution => 128
	i64 u0xf0ac2b489fed2e35, ; 479: lib_System.Diagnostics.Debug.dll.so => 109
	i64 u0xf0de2537ee19c6ca, ; 480: lib_System.Net.WebHeaderCollection.dll.so => 134
	i64 u0xf11b621fc87b983f, ; 481: Microsoft.Maui.Controls.Xaml.dll => 59
	i64 u0xf1c4b4005493d871, ; 482: System.Formats.Asn1.dll => 115
	i64 u0xf238bd79489d3a96, ; 483: lib-nl-Microsoft.Maui.Controls.resources.dll.so => 19
	i64 u0xf37221fda4ef8830, ; 484: lib_Xamarin.Google.Android.Material.dll.so => 91
	i64 u0xf3ddfe05336abf29, ; 485: System => 166
	i64 u0xf408654b2a135055, ; 486: System.Reflection.Emit.ILGeneration.dll => 140
	i64 u0xf4c1dd70a5496a17, ; 487: System.IO.Compression => 117
	i64 u0xf5fc7602fe27b333, ; 488: System.Net.WebHeaderCollection => 134
	i64 u0xf6077741019d7428, ; 489: Xamarin.AndroidX.CoordinatorLayout => 70
	i64 u0xf77b20923f07c667, ; 490: de/Microsoft.Maui.Controls.resources.dll => 4
	i64 u0xf7e2cac4c45067b3, ; 491: lib_System.Numerics.Vectors.dll.so => 135
	i64 u0xf7e74930e0e3d214, ; 492: zh-HK/Microsoft.Maui.Controls.resources.dll => 31
	i64 u0xf7fa0bf77fe677cc, ; 493: Newtonsoft.Json.dll => 63
	i64 u0xf84773b5c81e3cef, ; 494: lib-uk-Microsoft.Maui.Controls.resources.dll.so => 29
	i64 u0xf8b77539b362d3ba, ; 495: lib_System.Reflection.Primitives.dll.so => 143
	i64 u0xf8e045dc345b2ea3, ; 496: lib_Xamarin.AndroidX.RecyclerView.dll.so => 85
	i64 u0xf915dc29808193a1, ; 497: System.Web.HttpUtility.dll => 162
	i64 u0xf95306fe01fadbd0, ; 498: itext.commons.dll => 47
	i64 u0xf96c777a2a0686f4, ; 499: hi/Microsoft.Maui.Controls.resources.dll => 10
	i64 u0xf9eec5bb3a6aedc6, ; 500: Microsoft.Extensions.Options => 56
	i64 u0xfa0e82300e67f913, ; 501: lib_System.AppContext.dll.so => 98
	i64 u0xfa3f278f288b0e84, ; 502: lib_System.Net.Security.dll.so => 132
	i64 u0xfa5ed7226d978949, ; 503: lib-ar-Microsoft.Maui.Controls.resources.dll.so => 0
	i64 u0xfa645d91e9fc4cba, ; 504: System.Threading.Thread => 160
	i64 u0xfae3bcd3a0b1572a, ; 505: lib_itext.layout.dll.so => 42
	i64 u0xfbf0a31c9fc34bc4, ; 506: lib_System.Net.Http.dll.so => 127
	i64 u0xfc6b7527cc280b3f, ; 507: lib_System.Runtime.Serialization.Formatters.dll.so => 150
	i64 u0xfc719aec26adf9d9, ; 508: Xamarin.AndroidX.Navigation.Fragment.dll => 82
	i64 u0xfcd302092ada6328, ; 509: System.IO.MemoryMappedFiles.dll => 120
	i64 u0xfd22f00870e40ae0, ; 510: lib_Xamarin.AndroidX.DrawerLayout.dll.so => 74
	i64 u0xfd49b3c1a76e2748, ; 511: System.Runtime.InteropServices.RuntimeInformation => 146
	i64 u0xfd536c702f64dc47, ; 512: System.Text.Encoding.Extensions => 155
	i64 u0xfd583f7657b6a1cb, ; 513: Xamarin.AndroidX.Fragment => 75
	i64 u0xfdbe4710aa9beeff, ; 514: CommunityToolkit.Maui => 35
	i64 u0xfeae9952cf03b8cb ; 515: tr/Microsoft.Maui.Controls.resources => 28
], align 16

@assembly_image_cache_indices = dso_local local_unnamed_addr constant [516 x i32] [
	i32 88, i32 83, i32 36, i32 170, i32 66, i32 24, i32 2, i32 30,
	i32 130, i32 85, i32 103, i32 60, i32 31, i32 163, i32 69, i32 24,
	i32 101, i32 143, i32 74, i32 45, i32 56, i32 101, i32 153, i32 142,
	i32 159, i32 25, i32 95, i32 89, i32 21, i32 171, i32 61, i32 87,
	i32 128, i32 73, i32 116, i32 141, i32 85, i32 8, i32 169, i32 9,
	i32 52, i32 119, i32 53, i32 122, i32 64, i32 144, i32 167, i32 12,
	i32 156, i32 95, i32 18, i32 99, i32 166, i32 27, i32 170, i32 87,
	i32 84, i32 16, i32 56, i32 116, i32 111, i32 152, i32 140, i32 27,
	i32 122, i32 160, i32 118, i32 107, i32 71, i32 151, i32 8, i32 92,
	i32 93, i32 57, i32 123, i32 13, i32 11, i32 92, i32 169, i32 109,
	i32 130, i32 29, i32 129, i32 112, i32 44, i32 7, i32 158, i32 115,
	i32 33, i32 43, i32 20, i32 154, i32 141, i32 138, i32 161, i32 26,
	i32 157, i32 119, i32 5, i32 111, i32 164, i32 75, i32 34, i32 68,
	i32 113, i32 46, i32 8, i32 164, i32 100, i32 6, i32 133, i32 60,
	i32 2, i32 58, i32 90, i32 49, i32 145, i32 141, i32 120, i32 143,
	i32 123, i32 100, i32 73, i32 128, i32 89, i32 1, i32 63, i32 38,
	i32 155, i32 93, i32 163, i32 71, i32 81, i32 67, i32 38, i32 167,
	i32 171, i32 20, i32 97, i32 151, i32 93, i32 112, i32 98, i32 53,
	i32 24, i32 163, i32 22, i32 120, i32 136, i32 84, i32 145, i32 157,
	i32 80, i32 129, i32 124, i32 139, i32 148, i32 14, i32 80, i32 170,
	i32 154, i32 40, i32 159, i32 1, i32 40, i32 58, i32 37, i32 41,
	i32 78, i32 114, i32 130, i32 108, i32 44, i32 71, i32 62, i32 25,
	i32 129, i32 146, i32 31, i32 48, i32 152, i32 76, i32 102, i32 137,
	i32 168, i32 110, i32 15, i32 51, i32 97, i32 70, i32 161, i32 106,
	i32 3, i32 54, i32 134, i32 147, i32 69, i32 102, i32 156, i32 104,
	i32 164, i32 108, i32 5, i32 51, i32 94, i32 126, i32 123, i32 59,
	i32 4, i32 148, i32 168, i32 43, i32 100, i32 91, i32 144, i32 35,
	i32 58, i32 149, i32 118, i32 107, i32 78, i32 72, i32 3, i32 113,
	i32 115, i32 9, i32 96, i32 147, i32 18, i32 62, i32 57, i32 72,
	i32 57, i32 82, i32 96, i32 60, i32 2, i32 119, i32 28, i32 18,
	i32 14, i32 104, i32 11, i32 126, i32 49, i32 86, i32 149, i32 17,
	i32 27, i32 75, i32 98, i32 7, i32 105, i32 25, i32 4, i32 36,
	i32 17, i32 135, i32 103, i32 136, i32 106, i32 89, i32 50, i32 77,
	i32 142, i32 166, i32 33, i32 66, i32 68, i32 114, i32 29, i32 42,
	i32 47, i32 32, i32 122, i32 33, i32 49, i32 160, i32 116, i32 61,
	i32 144, i32 94, i32 167, i32 104, i32 146, i32 80, i32 46, i32 110,
	i32 111, i32 9, i32 72, i32 161, i32 99, i32 63, i32 81, i32 10,
	i32 23, i32 22, i32 21, i32 112, i32 34, i32 117, i32 78, i32 59,
	i32 73, i32 157, i32 125, i32 1, i32 17, i32 117, i32 6, i32 13,
	i32 62, i32 106, i32 99, i32 124, i32 37, i32 83, i32 16, i32 65,
	i32 50, i32 19, i32 109, i32 81, i32 64, i32 77, i32 41, i32 153,
	i32 92, i32 91, i32 84, i32 121, i32 16, i32 37, i32 108, i32 150,
	i32 135, i32 153, i32 48, i32 86, i32 87, i32 74, i32 76, i32 12,
	i32 36, i32 55, i32 139, i32 127, i32 52, i32 5, i32 124, i32 149,
	i32 77, i32 165, i32 23, i32 159, i32 19, i32 162, i32 105, i32 132,
	i32 169, i32 136, i32 42, i32 79, i32 26, i32 158, i32 45, i32 3,
	i32 68, i32 10, i32 0, i32 121, i32 39, i32 48, i32 55, i32 45,
	i32 113, i32 26, i32 168, i32 97, i32 22, i32 15, i32 41, i32 165,
	i32 103, i32 133, i32 150, i32 39, i32 127, i32 90, i32 44, i32 67,
	i32 0, i32 107, i32 125, i32 65, i32 15, i32 90, i32 53, i32 79,
	i32 105, i32 131, i32 82, i32 139, i32 137, i32 96, i32 155, i32 162,
	i32 102, i32 35, i32 28, i32 20, i32 23, i32 34, i32 158, i32 94,
	i32 95, i32 32, i32 131, i32 46, i32 88, i32 110, i32 138, i32 142,
	i32 40, i32 145, i32 76, i32 43, i32 147, i32 67, i32 137, i32 54,
	i32 14, i32 52, i32 79, i32 86, i32 88, i32 64, i32 50, i32 55,
	i32 47, i32 51, i32 101, i32 118, i32 30, i32 140, i32 30, i32 125,
	i32 54, i32 152, i32 156, i32 32, i32 165, i32 11, i32 66, i32 70,
	i32 121, i32 148, i32 138, i32 132, i32 131, i32 13, i32 21, i32 12,
	i32 7, i32 151, i32 171, i32 38, i32 114, i32 133, i32 126, i32 83,
	i32 69, i32 154, i32 39, i32 61, i32 65, i32 6, i32 128, i32 109,
	i32 134, i32 59, i32 115, i32 19, i32 91, i32 166, i32 140, i32 117,
	i32 134, i32 70, i32 4, i32 135, i32 31, i32 63, i32 29, i32 143,
	i32 85, i32 162, i32 47, i32 10, i32 56, i32 98, i32 132, i32 0,
	i32 160, i32 42, i32 127, i32 150, i32 82, i32 120, i32 74, i32 146,
	i32 155, i32 75, i32 35, i32 28
], align 16

@marshal_methods_number_of_classes = dso_local local_unnamed_addr constant i32 0, align 4

@marshal_methods_class_cache = dso_local local_unnamed_addr global [0 x %struct.MarshalMethodsManagedClass] zeroinitializer, align 8

; Names of classes in which marshal methods reside
@mm_class_names = dso_local local_unnamed_addr constant [0 x ptr] zeroinitializer, align 8

@mm_method_names = dso_local local_unnamed_addr constant [1 x %struct.MarshalMethodName] [
	%struct.MarshalMethodName {
		i64 u0x0000000000000000, ; name: 
		ptr @.MarshalMethodName.0_name; char* name
	} ; 0
], align 8

; get_function_pointer (uint32_t mono_image_index, uint32_t class_index, uint32_t method_token, void*& target_ptr)
@get_function_pointer = internal dso_local unnamed_addr global ptr null, align 8

; Functions

; Function attributes: memory(write, argmem: none, inaccessiblemem: none) "min-legal-vector-width"="0" mustprogress "no-trapping-math"="true" nofree norecurse nosync nounwind "stack-protector-buffer-size"="8" uwtable willreturn
define void @xamarin_app_init(ptr nocapture noundef readnone %env, ptr noundef %fn) local_unnamed_addr #0
{
	%fnIsNull = icmp eq ptr %fn, null
	br i1 %fnIsNull, label %1, label %2

1: ; preds = %0
	%putsResult = call noundef i32 @puts(ptr @.str.0)
	call void @abort()
	unreachable 

2: ; preds = %1, %0
	store ptr %fn, ptr @get_function_pointer, align 8, !tbaa !3
	ret void
}

; Strings
@.str.0 = private unnamed_addr constant [40 x i8] c"get_function_pointer MUST be specified\0A\00", align 16

;MarshalMethodName
@.MarshalMethodName.0_name = private unnamed_addr constant [1 x i8] c"\00", align 1

; External functions

; Function attributes: "no-trapping-math"="true" noreturn nounwind "stack-protector-buffer-size"="8"
declare void @abort() local_unnamed_addr #2

; Function attributes: nofree nounwind
declare noundef i32 @puts(ptr noundef) local_unnamed_addr #1
attributes #0 = { memory(write, argmem: none, inaccessiblemem: none) "min-legal-vector-width"="0" mustprogress "no-trapping-math"="true" nofree norecurse nosync nounwind "stack-protector-buffer-size"="8" "target-cpu"="x86-64" "target-features"="+crc32,+cx16,+cx8,+fxsr,+mmx,+popcnt,+sse,+sse2,+sse3,+sse4.1,+sse4.2,+ssse3,+x87" "tune-cpu"="generic" uwtable willreturn }
attributes #1 = { nofree nounwind }
attributes #2 = { "no-trapping-math"="true" noreturn nounwind "stack-protector-buffer-size"="8" "target-cpu"="x86-64" "target-features"="+crc32,+cx16,+cx8,+fxsr,+mmx,+popcnt,+sse,+sse2,+sse3,+sse4.1,+sse4.2,+ssse3,+x87" "tune-cpu"="generic" }

; Metadata
!llvm.module.flags = !{!0, !1}
!0 = !{i32 1, !"wchar_size", i32 4}
!1 = !{i32 7, !"PIC Level", i32 2}
!llvm.ident = !{!2}
!2 = !{!".NET for Android remotes/origin/release/9.0.1xx @ 1dcfb6f8779c33b6f768c996495cb90ecd729329"}
!3 = !{!4, !4, i64 0}
!4 = !{!"any pointer", !5, i64 0}
!5 = !{!"omnipotent char", !6, i64 0}
!6 = !{!"Simple C++ TBAA"}
