using UnityEngine;
using System.Collections;
using nobnak.Texture;
using System.Threading;
using nobnak.Noise;
using nobnak.Timer;
using System.Collections.Generic;

namespace UneuneSpots {

	public class CPUHeightMap : MonoBehaviour {
		public enum DebugModeEnum { None = 0, OnGUI }

		public Vector4 texST = new Vector4(0.25f, 1f, 0f, 0f);
		public Vector4 size = new Vector4(0.5f, 1.0f, 2.0f, 4.0f);
		public Vector4 speed = new Vector4(0.003f, 0.003f, 0.003f, 0.003f);
		public Vector4 gain = new Vector4(0.37f, 4.28f, 1.72f, -2.45f);
		public Vector4 power = new Vector4(2.4f, 5.46f, 2.96f, 2.56f);

		public Circle mask;

		public CPUTexture NoiseMap { get; private set; }

		public int mipLevel = 3;
		public float generationInterval = 1f;

		public float debugScale = 1f;
		public DebugModeEnum debugMode;

		private double _t;
		private float _aspect;
		private int _width;
		private int _height;

		private System.Diagnostics.Stopwatch _timer;
		private IEnumerator _iterator;
		private volatile Thread _thread;

		private Texture2D _debugTex;
		private Color[] _debugPixels;

		void OnEnable() {
			_iterator = Generate();
			_timer = System.Diagnostics.Stopwatch.StartNew();
		}
		void OnDisable() {
			ReleaseTextures();
			if (_thread != null) {
				_thread.Interrupt();
				_thread = null;
			}
		}
		void Update() {
			if (!_iterator.MoveNext() && generationInterval < _timer.Elapsed.TotalSeconds) {
				_iterator = Generate();
				_timer.Reset();
				_timer.Start();
			}
			if (debugMode != DebugModeEnum.None && Input.GetMouseButton(0)) {
				var sspos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
				mask.offset = (Vector2)sspos;
			}
		}
		void OnGUI() {
			if (debugMode == DebugModeEnum.OnGUI && Event.current.type.Equals(EventType.Repaint)) {
				var screen = new Rect(0f, 0f, Screen.width, Screen.height);
				_debugTex.SetPixels(_debugPixels);
				_debugTex.Apply();
				Graphics.DrawTexture(screen, _debugTex);
			}
		}

		public IEnumerator Generate() {
			lock (this) {
				if (_thread != null)
					yield break;
				_thread = new Thread(Generator);
			}

			CheckTextures();
			_t = TimeUtil.Time2014;
			_thread.Start();
			while (_thread != null)
				yield return null;
		}

		void Generator(object state) {
			try {
				var dx = 1.0 / NoiseMap.Width;
				var dy = 1.0 / NoiseMap.Height;

				for (var y = 0; y < NoiseMap.Height; y++) {
					for (var x = 0; x < NoiseMap.Width; x++) {
						var u = x * dx * texST.x + texST.z;
						var v = y * dy * texST.y + texST.w;
						u *= _aspect;

						var v0 = SimplexNoise.noise(size.x * u, size.x * v, speed.x * _t);
						var v1 = SimplexNoise.noise(size.y * u, size.y * v, speed.y * _t);
						var v2 = SimplexNoise.noise(size.z * u, size.z * v, speed.z * _t);
						var v3 = SimplexNoise.noise(size.w * u, size.w * v, speed.w * _t);

						v0 = Mathf.Pow((float)(0.5 * (v0 + 1.0)), power.x);
						v1 = Mathf.Pow((float)(0.5 * (v1 + 1.0)), power.y);
						v2 = Mathf.Pow((float)(0.5 * (v2 + 1.0)), power.z);
						v3 = Mathf.Pow((float)(0.5 * (v3 + 1.0)), power.w);

						var h = (float)(gain.x * v0 + gain.y * v1 + gain.z * v2 + gain.w * v3);
						h = (h < 0f ? 0f : h);
						NoiseMap[x, y] = h * mask.Mask((float)(x * dx), (float)(y * dy));
					}
				}

				if (debugMode != DebugModeEnum.None) {
					for (var y = 0; y < _height; y++) {
						for (var x = 0; x < _width; x++) {
							var i = x + y * _width;
							var h = debugScale * NoiseMap[x, y];
							_debugPixels[i] = new Color(h, h, h, 1f);
						}
					}
				}
			} catch (System.Exception e) {
				Debug.Log(e);
			} finally {
				lock (this)
					_thread = null;
			}
		}

		void CheckTextures() {
			var sizeDiv = 1 << mipLevel;
			_width = Screen.width / sizeDiv;
			_height = Screen.height / sizeDiv;
			_aspect = (float)Screen.width / Screen.height;

			if (NoiseMap == null || NoiseMap.Width != _width || NoiseMap.Height != _height) {
				ReleaseTextures();
				NoiseMap = new CPUTexture(_width, _height);
				_debugTex = new Texture2D(_width, _height, TextureFormat.RGB24, false);
				_debugPixels = _debugTex.GetPixels();
			}
		}

		void ReleaseTextures() {
			Destroy(_debugTex);
		}
	}

	[System.Serializable]
	public class Circle {
		public Vector2 size = new Vector2(0.1f, 0.1f);
		public Vector2 center = new Vector2(0.5f, 0.5f);
		public Vector2 offset = Vector2.zero;
		
		public float Mask(float u, float v) {
			var rx = (u - center.x - offset.x) / size.x;
			var ry = (v - center.y - offset.y) / size.y;
			var sqDist = rx * rx + ry * ry;
			if (sqDist <= 1f)
				return 1f - sqDist;
			else
				return 0f;
		}
	}
}
