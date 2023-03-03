## RenderTexture
### OnRenderImage
实现了OnRenderImage方法，会增加一份名为 "TempBuffer 1 1080x2340" 的缓存文件。[（官方描述：internally, Unity creates one or more temporary RenderTextures to store these intermediate results.）](https://docs.unity.cn/cn/current/ScriptReference/MonoBehaviour.OnRenderImage.html)
![][1677826512905.jpg]
</br></br>
RenderTexture 用完一定要Release</br>
new出来的用 rt.Release();</br>
通过RenderTexture.GetTemporary()获取的调用RenderTexture.ReleaseTemporary(rt); </br>
