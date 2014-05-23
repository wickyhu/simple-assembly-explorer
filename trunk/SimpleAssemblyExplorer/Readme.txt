
Simple Assembly Explorer by WiCKY Hu
http://code.google.com/p/simple-assembly-explorer/



Todo/Known Issues List:

==Mono.Cecil:====================================================

1. AssemblyReader.GetMemberReference and Generic case
http://groups.google.com/group/mono-cecil/browse_thread/thread/f4088687d73e87a4/a168740fa6744f8d

Because I don't want to search all instructions to handle generic case (it will be slow),
I patched the function to ignore the generic checking, so Cecil 0.9 save member references like 0.6,
which is actually a "flaw"?


==Deobfuscator:==================================================

1. Name option: 
for complex case, IL may be correct but C# will be wrong because override information lost
For example:
interface I1 { methodA }
interface I2 { methodA }
interface I3 { methodA }
class C1 : I1, I2, I3
{
	methodA implement I1.methodA
	methodB implement I2.methodA
	methodC implement I3.methodA
}
methodB and methodC can't be renamed to methodA directly.
need to check whole inheritance/implementation tree


2. Flow option: 
currently doesn't consider much about exception handler, so instructions may be moved in or out exception handler,
which could make C# can't be compiled


3. Flow option: 
best way to find out logical executing sequence of instructions?


4. Flow option:
For dynamic methods, Name + Delegate Call may left some obfuscated names
http://code.google.com/p/simple-assembly-explorer/issues/detail?id=87


5. Name option, Boolean Function option, Dynamic Method option

5.1) These options call methods, so it could hang SAE if method doesn't return.
Is it possible to call them asynchronously? Is there performance issue?

5.2) These options call methods, so original assembly is loaded into current appdomain and can't be unloaded.
Is it possible to load them in another appdomain? Is there performance issue?
