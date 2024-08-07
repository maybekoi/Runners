# SRU18 

* A merge of TyDev's and mattkc's Sonic Runners Decomps but also a port of the decomps to Unity 2018

# How to setup SRU18

By: SnesFX/Blitzdotcs

1. Download files from this repo using
`git clone https://github.com/blitzdotcs/SRU18.git `

2. Download the files from [here](https://github.com/itsmattkc/RunnersAssetBundleDecomp)

3. Open the zip and Open the RunnersAssetBundleDecomp-master folder

4. Extract the files

5. Go to wherever you extracted the decomp and go into the Assets folder

6. Make a folder called AssetBundles

7. Paste Everything there

8. Download Unity 2018.2.19f1 if you don't have it

9. Open the project in Unity

10. Build or Download [Outrun](https://github.com/fluofoxxo/outrun)

11. Navigate to Assets/Scripts/ and open NetBaseUtil.cs with VSC
    
12. Find the variable `mActionServerUrlTable `
    
13. Edit every string in the `mActionServerUrlTable` array to `http://<IP>:<PORT>/` where `<IP>` is replaced by the IP for your instance and `<PORT>` is replaced by the port for your instance (Default: 9001)
    
14. Repeat step 14 for `mSecureActionServerUrlTable`
    
15. Click File -> Save File
    
16. Enjoy!

# Note

* This is still a work in progress and this is being done by one person only, so expect development to be slow.

# Credits

* BlitzEX - Owner

* TyDev - Decomp, Porter, and original owner

* MattKc - Decomp
