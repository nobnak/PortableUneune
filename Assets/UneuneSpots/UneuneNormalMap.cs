using UnityEngine;
using System.Collections;

namespace UneuneSpots {

	public class UneuneNormalMap : MonoBehaviour {

		public Material heightGenMat;
		public Material normalGenMat;
		public Material heightViewMat;
		public Material normalViewMat;

		public int mipLevel = 1;
		public float height = 0.5f;
		public KeyCode debugKey = KeyCode.N;

		public RenderTexture _heightRTex;
		public RenderTexture _normalRTex;

		private int _debugMode = 0;

		void OnDestroy() {
			Clear();
		}
		void Update() {
			if (Input.GetKeyDown(debugKey)) {
				_debugMode = ++_debugMode % 3;
				Screen.showCursor = (_debugMode != 0);
			}
			ManualUpdate();
		}
		void OnGUI() {
			if (_debugMode != 0 && _heightRTex != null && Event.current.type.Equals(EventType.Repaint)) {
				var fill = new Rect(0f, 0f, Screen.width, Screen.height);
				switch (_debugMode) {
				case 1:
					Graphics.DrawTexture(fill, _heightRTex, heightViewMat);
					break;
				case 2:
					Graphics.DrawTexture(fill, _normalRTex, normalViewMat);
					break;
				}
			}
		}

		public void ManualUpdate() {
			CheckTexture();
			Graphics.Blit(null, _heightRTex, heightGenMat);
			Graphics.Blit(_heightRTex, _normalRTex, normalGenMat);

			var time = TimeUtil.ShaderTime2014;
			Shader.SetGlobalVector(ShaderConst.DATE_TIME, time);
			normalGenMat.SetFloat(ShaderConst.UNEUNE_HEIGHT, height);
			Shader.SetGlobalFloat(ShaderConst.UNEUNE_HEIGHT, height);
			Shader.SetGlobalTexture(ShaderConst.UNEUNE_HEIGHT_MAP, _heightRTex);
			Shader.SetGlobalTexture(ShaderConst.UNEUNE_NORMAL_MAP, _normalRTex);

		}
		
		void CheckTexture() {
			var sizeDiv = 1 << mipLevel;
			var width = Screen.width / sizeDiv;
			var height = Screen.height / sizeDiv;

			if (_heightRTex == null || _heightRTex.width != width || _heightRTex.height != height) {
				Clear();

				_heightRTex = new RenderTexture(width, height, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
				_normalRTex = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
				_heightRTex.wrapMode = TextureWrapMode.Clamp;
				_normalRTex.wrapMode = TextureWrapMode.Clamp;
				Debug.Log(string.Format("Create Uneune NormalMap {0}x{1}", width, height));
			}
		}
		void Clear() {
			Destroy(_heightRTex);
			Destroy(_normalRTex);
		}
	}
}