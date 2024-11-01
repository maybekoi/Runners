# SRU18 

* A merge of me and mattkc's Sonic Runners Decomps but also a port of the decomps to Unity 2018

# How to setup SRU18

By: SnesFX/YoPhlox

1. Download files from this repo using
`git clone https://github.com/yophlox/SRU18.git --recursive`

2. Download Unity 2018.2.19f1 if you don't have it

3. Open the project in Unity

4. Build or Download [Outrun](https://github.com/fluofoxxo/outrun)

5. Navigate to Assets/Scripts/ and open NetBaseUtil.cs with VSC
    
6. Find the variable `mActionServerUrlTable `
    
7. Edit every string in the `mActionServerUrlTable` array to `http://<IP>:<PORT>/` where `<IP>` is replaced by the IP for your instance and `<PORT>` is replaced by the port for your instance (Default: 9001)
    
8. Repeat step 7 for `mSecureActionServerUrlTable`
    
9. Click File -> Save File
    
10. Enjoy!

# Note

* This is still a work in progress and this is being done by one person only, so expect development to be slow.

# Credits

* YoPhlox - Decomp & Porter

* MattKc - Decomp
