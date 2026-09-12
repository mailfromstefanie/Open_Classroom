using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDK3.Image;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class PosterImageLoaderLocal : UdonSharpBehaviour
{
    [Header("Image Loading")]
    public VRCUrlInputField urlInput;
    public Material posterMaterial;

    [Header("Emission")]
    public string emissionTextureProperty = "_EmissionMap";
    public string emissionColorProperty = "_EmissionColor";
    public Color emissionColor = Color.white;

    private VRCImageDownloader imageDownloader;
    private Texture defaultMainTexture;

    private void Start()
    {
        imageDownloader = new VRCImageDownloader();

        if (posterMaterial != null)
        {
            defaultMainTexture = posterMaterial.mainTexture;
        }

        SetEmission(false);
    }

    public void LoadImage()
    {
        VRCUrl url = urlInput.GetUrl();

        if (VRCUrl.IsNullOrEmpty(url))
        {
            Debug.LogWarning("[Poster] No image URL entered.");
            return;
        }

        LoadUrl(url);
    }

    public void LoadUrl(VRCUrl url)
    {
        if (VRCUrl.IsNullOrEmpty(url))
        {
            Debug.LogWarning("[Poster] No image URL to load.");
            return;
        }

        imageDownloader.DownloadImage(
            url,
            posterMaterial,
            (IUdonEventReceiver)this
        );
    }

    public override void OnImageLoadSuccess(IVRCImageDownload result)
    {
        if (posterMaterial != null &&
            posterMaterial.HasProperty(emissionTextureProperty))
        {
            posterMaterial.SetTexture(
                emissionTextureProperty,
                result.Result
            );
        }

        SetEmission(true);
        Debug.Log("[Poster] Image loaded successfully.");
    }

    public override void OnImageLoadError(IVRCImageDownload result)
    {
        // Keep the last successful poster visible if a replacement URL fails.
        Debug.LogError(
            "[Poster] Image load failed: " +
            result.ErrorMessage
        );
    }

    public void UnloadImage()
    {
        if (posterMaterial == null)
            return;

        posterMaterial.mainTexture = defaultMainTexture;

        if (posterMaterial.HasProperty(emissionTextureProperty))
        {
            posterMaterial.SetTexture(
                emissionTextureProperty,
                null
            );
        }

        SetEmission(false);
        Debug.Log("[Poster] Poster unloaded.");
    }

    private void SetEmission(bool enabled)
    {
        if (posterMaterial == null)
            return;

        if (!posterMaterial.HasProperty(emissionColorProperty))
            return;

        if (enabled)
        {
            posterMaterial.SetColor(
                emissionColorProperty,
                emissionColor
            );

            posterMaterial.EnableKeyword("_EMISSION");
        }
        else
        {
            posterMaterial.SetColor(
                emissionColorProperty,
                Color.black
            );

            posterMaterial.DisableKeyword("_EMISSION");
        }
    }
}
