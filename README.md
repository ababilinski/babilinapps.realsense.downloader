# Intel RealSense Package Downloader 

**A simple package that downloads the newest RealSense Unity Package from the Official Release Page.**

## Why was it created?

I have been using the Intel RealSense SDK for a lot of my projects and wanted to have a way to download the RealSense package from the Unity Packages window. 

How to install the package
--------------------------
1. This package can be installed by adding the Git url to the package manager

2. This package can be installed using the [scoped registry] feature

## Scoped Registry How To
Please add the following sections to the package manifest file (`Packages/manifest.json`).

To the `scopedRegistries` section:

```

{
  "name": "babilinapps",
  "url": "https://registry.npmjs.com",
  "scopes": [ "babilinapps.realsense" ]
}
```

To the `dependencies` section:

```
"babilinapps.realsense.downloader": "0.1.0",
```

After changes, the manifest file should look like below:

```
{
  "scopedRegistries": [
    {
	  "name": "babilinapps",
	  "url": "https://registry.npmjs.com",
	  "scopes": [ "babilinapps.realsense" ]
    }
  ],
  "dependencies": {
    "babilinapps.realsense.downloader": "0.1.0",
...
```

[scoped registry]: https://docs.unity3d.com/Manual/upm-scoped.html
 

## How can the package be Improved?

Ideally, this package would not be required and RealSense would update their repo to support the new Unity Package System. Until then, here are a list of item that still need to be done:
- Use the GitHub API to download the package and release notes.
- Show an update notification if the a new version was released.

## Acknowledgements
Thank you to **Keijiro Takahashikeijiro** ([@keijiro]) for your open source projects. The idea of adding this package to was based off of their project structure. 

[@keijiro]: https://github.com/keijiro
