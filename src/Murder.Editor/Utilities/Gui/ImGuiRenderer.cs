using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Murder.Assets;
using Murder.Data;
using Murder.Diagnostics;
using Murder.Serialization;
using Murder.Utilities;
using System.Runtime.InteropServices;

namespace Murder.Editor.ImGuiExtended
{
    /// <summary>
    /// ImGui renderer for use with XNA-likes (FNA and MonoGame)
    /// Stolen from https://github.com/mellinoe/ImGui.NET/tree/master/src/ImGui.NET.SampleProgram.XNA
    /// </summary>
    public class ImGuiRenderer : IDisposable
    {
        private readonly Microsoft.Xna.Framework.Game _game;

        // Graphics
        private readonly GraphicsDevice _graphicsDevice;

        private BasicEffect? _effect;
        private readonly RasterizerState _rasterizerState;

        private byte[]? _vertexData;
        private VertexBuffer? _vertexBuffer;
        private int _vertexBufferSize;

        private byte[]? _indexData;
        private IndexBuffer? _indexBuffer;
        private int _indexBufferSize;

        // Textures
        private readonly Dictionary<IntPtr, Texture2D> _loadedTextures = new();

        private int _textureId;
        private IntPtr? _fontTextureId;

        // Input
        private int _scrollWheelValue;

        private readonly List<int> _keys = new List<int>();

        public ImGuiRenderer(Microsoft.Xna.Framework.Game game)
        {
            var context = ImGui.CreateContext();
            ImGui.SetCurrentContext(context);

            _game = game ?? throw new ArgumentNullException(nameof(game));
            _graphicsDevice = game.GraphicsDevice;

            _rasterizerState = new RasterizerState()
            {
                CullMode = CullMode.None,
                DepthBias = 0,
                FillMode = FillMode.Solid,
                MultiSampleAntiAlias = false,
                ScissorTestEnable = true,
                SlopeScaleDepthBias = 0
            };

            SetupInput();
            
            AddFonts();
            //var io = ImGui.GetIO();
            //Editor.EditorScene.FaFont = io.Fonts.AddFontFromFileTTF("Font Awesome 6 Free-Solid-900.otf", 14);
            //Editor.EditorScene.EditorFont = io.Fonts.AddFontFromFileTTF("JetBrainsMono-Regular.ttf", 12);
            //io.Fonts.Build();
        }

        void AddFonts()
        {
            var io = ImGui.GetIO();

            //unsafe void loadFont(string path, float size)
            //{
            //    var fontData = File.ReadAllBytes(path);
            //    fixed (byte* ptr = fontData)
            //    {
            //        io.Fonts.AddFontFromMemoryTTF((IntPtr)ptr, fontData.Length, size);
            //    }
            //}
            
            //unsafe ImFontPtr loadIconFont(string path, float size)
            //{
            //    var fontData = File.ReadAllBytes(FileHelper.GetPath(path));
            //    ImFontPtr font = null;
            //    fixed (byte* ptr = fontData)
            //    {
            //        var config = ImGuiNative.ImFontConfig_ImFontConfig();
            //        config->MergeMode = 1;
            //        config->GlyphMinAdvanceX = size;

            //        var ranges = new ushort[] { FontAwesome.IconMin, FontAwesome.IconMax, 0 };
            //        fixed (ushort* rangesPtr = ranges)
            //        {
            //            font = io.Fonts.AddFontFromMemoryTTF((IntPtr)ptr, fontData.Length, size, config, (IntPtr)rangesPtr);
            //        }

            //        ImGuiNative.ImFontConfig_destroy(config);
            //    }

            //    return font;
            //}

            unsafe
            {
                var config = ImGuiNative.ImFontConfig_ImFontConfig();
                config->MergeMode = 1;
                config->GlyphMinAdvanceX = 14;

                io.Fonts.AddFontDefault(config);
                var ranges = new ushort[] { FontAwesome.IconMin, FontAwesome.IconMax, 0 };

                fixed (ushort* rangesPtr = ranges)
                {
                    void AddFont(string fontName, IntPtr r)
                    {
                        string path = FileHelper.GetPath("resources","fonts", fontName);
                        if (!File.Exists(path)){
                            GameLogger.Error($"ImGui font couldn't be found at {path}, using default.");
                        }
                        else
                        {
                            var io = ImGui.GetIO();
                            io.Fonts.AddFontFromFileTTF(path, 12, config, r);
                        }
                    }
                    AddFont("fa-regular-400.otf", (IntPtr)rangesPtr);
                    AddFont("fa-solid-400.otf", (IntPtr)rangesPtr);
                }
                
                ImGuiNative.ImFontConfig_destroy(config);
            }


            io.Fonts.Build();
        }

        #region ImGuiRenderer

        /// <summary>
        /// Creates a texture and loads the font data from ImGui. Should be called when the <see cref="GraphicsDevice" /> is initialized but before any rendering is done
        /// </summary>
        public virtual unsafe void RebuildFontAtlas()
        {
            // Get font texture from ImGui
            var io = ImGui.GetIO();
            io.Fonts.GetTexDataAsRGBA32(out byte* pixelData, out int width, out int height, out int bytesPerPixel);

            // Copy the data to a managed array
            var pixels = new byte[width * height * bytesPerPixel];
            unsafe { Marshal.Copy(new IntPtr(pixelData), pixels, 0, pixels.Length); }

            // Create and register the texture as an XNA texture
            var tex2d = new Texture2D(_graphicsDevice, width, height, false, SurfaceFormat.Color);
            tex2d.SetData(pixels);

            // Should a texture already have been build previously, unbind it first so it can be deallocated
            if (_fontTextureId.HasValue) UnbindTexture(_fontTextureId.Value);

            // Bind the new texture to an ImGui-friendly id
            _fontTextureId = BindTexture(tex2d);

            // Let ImGui know where to find the texture
            io.Fonts.SetTexID(_fontTextureId.Value);
            io.Fonts.ClearTexData(); // Clears CPU side texture data
        }

        /// <summary>
        /// Creates a pointer to a texture, which can be passed through ImGui calls such as <see cref="ImGui.Image(nint, System.Numerics.Vector2)" />. That pointer is then used by ImGui to let us know what texture to draw
        /// </summary>
        public virtual IntPtr BindTexture(Texture2D texture)
        {
            var id = GetNextIntPtr();
            int threshold = 1000;
            if (_loadedTextures.Count % threshold == 0)
            {
                GameLogger.Warning($"{nameof(ImGuiRenderer)}: You have loaded {_loadedTextures.Count} textures. This may cause performance issues. Consider unloading unused textures.");
            }

            _loadedTextures.Add(id, texture);

            texture.Disposing += (o, e) =>
            {
                _loadedTextures.Remove(id);
            };

            return id;
        }


        public Texture2D? GetLoadedTexture(IntPtr id)
        {
            if (_loadedTextures.TryGetValue(id, out var oldTexture))
            {
                return oldTexture;
            }

            return null;
        }

        /// <summary>
        /// Creates a pointer to a texture, which can be passed through ImGui calls such as <see cref="ImGui.Image(nint, System.Numerics.Vector2)" />. That pointer is then used by ImGui to let us know what texture to draw
        /// </summary>
        public virtual IntPtr BindTexture(IntPtr id, Texture2D texture, bool unloadPrevious)
        {
            if (unloadPrevious && _loadedTextures.TryGetValue(id, out var oldTexture))
            {
                oldTexture.Dispose();
            }

            _loadedTextures[id] = texture;

            return id;
        }

        public IntPtr GetNextIntPtr() => new IntPtr(_textureId++);

        /// <summary>
        /// Removes a previously created texture pointer, releasing its reference and allowing it to be deallocated
        /// </summary>
        public virtual void UnbindTexture(IntPtr textureId)
        {
            _loadedTextures[textureId].Dispose();
        }

        /// <summary>
        /// Sets up ImGui for a new frame, should be called at frame start
        /// </summary>
        public virtual void BeforeLayout(GameTime gameTime)
        {
            ImGui.GetIO().DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            UpdateInput();

            ImGui.NewFrame();
        }

        /// <summary>
        /// Asks ImGui for the generated geometry data and sends it to the graphics pipeline, should be called after the UI is drawn using ImGui.** calls
        /// </summary>
        public virtual void AfterLayout()
        {
            ImGui.Render();

            unsafe { RenderDrawData(ImGui.GetDrawData()); }
        }

        #endregion ImGuiRenderer

        #region Setup & Update

        /// <summary>
        /// Maps ImGui keys to XNA keys. We use this later on to tell ImGui what keys were pressed
        /// </summary>
        protected virtual void SetupInput()
        {
            var io = ImGui.GetIO();

            _keys.Add(io.KeyMap[(int)ImGuiKey.Tab] = (int)Keys.Tab);
            _keys.Add(io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)Keys.Left);
            _keys.Add(io.KeyMap[(int)ImGuiKey.RightArrow] = (int)Keys.Right);
            _keys.Add(io.KeyMap[(int)ImGuiKey.UpArrow] = (int)Keys.Up);
            _keys.Add(io.KeyMap[(int)ImGuiKey.DownArrow] = (int)Keys.Down);
            _keys.Add(io.KeyMap[(int)ImGuiKey.PageUp] = (int)Keys.PageUp);
            _keys.Add(io.KeyMap[(int)ImGuiKey.PageDown] = (int)Keys.PageDown);
            _keys.Add(io.KeyMap[(int)ImGuiKey.Home] = (int)Keys.Home);
            _keys.Add(io.KeyMap[(int)ImGuiKey.End] = (int)Keys.End);
            _keys.Add(io.KeyMap[(int)ImGuiKey.Delete] = (int)Keys.Delete);
            _keys.Add(io.KeyMap[(int)ImGuiKey.Backspace] = (int)Keys.Back);
            _keys.Add(io.KeyMap[(int)ImGuiKey.Enter] = (int)Keys.Enter);
            _keys.Add(io.KeyMap[(int)ImGuiKey.Escape] = (int)Keys.Escape);
            _keys.Add(io.KeyMap[(int)ImGuiKey.Space] = (int)Keys.Space);
            _keys.Add(io.KeyMap[(int)ImGuiKey.A] = (int)Keys.A);
            _keys.Add(io.KeyMap[(int)ImGuiKey.C] = (int)Keys.C);
            _keys.Add(io.KeyMap[(int)ImGuiKey.V] = (int)Keys.V);
            _keys.Add(io.KeyMap[(int)ImGuiKey.X] = (int)Keys.X);
            _keys.Add(io.KeyMap[(int)ImGuiKey.Y] = (int)Keys.Y);
            _keys.Add(io.KeyMap[(int)ImGuiKey.Z] = (int)Keys.Z);

            // MonoGame-specific //////////////////////
            _game.Window.TextInput += (s, a) =>
            {
                if (a.Character == '\t') return;

                io.AddInputCharacter(a.Character);
            };
            ///////////////////////////////////////////

            // FNA-specific ///////////////////////////
            //TextInputEXT.TextInput += c =>
            //{
            //    if (c == '\t') return;

            //    ImGui.GetIO().AddInputCharacter(c);
            //};
            ///////////////////////////////////////////

            ImGui.GetIO().Fonts.AddFontDefault();
        }

        /// <summary>
        /// Updates the <see cref="Effect" /> to the current matrices and texture
        /// </summary>
        protected virtual Effect UpdateEffect(Texture2D texture)
        {
            _effect = _effect ?? new BasicEffect(_graphicsDevice);

            var io = ImGui.GetIO();

            _effect.World = Matrix.Identity;
            _effect.View = Matrix.Identity;
            _effect.Projection = Matrix.CreateOrthographicOffCenter(0f, io.DisplaySize.X, io.DisplaySize.Y, 0f, -1f, 1f);
            _effect.TextureEnabled = true;
            _effect.Texture = texture;
            _effect.VertexColorEnabled = true;

            return _effect;
        }

        /// <summary>
        /// Sends XNA input state to ImGui
        /// </summary>
        protected virtual void UpdateInput()
        {
            var io = ImGui.GetIO();

            var mouse = Mouse.GetState();
            var keyboard = Keyboard.GetState();

            for (int i = 0; i < _keys.Count; i++)
            {
                io.KeysDown[_keys[i]] = keyboard.IsKeyDown((Keys)_keys[i]);
            }

            io.KeyShift = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);
            io.KeyCtrl = keyboard.IsKeyDown(Keys.LeftControl) || keyboard.IsKeyDown(Keys.RightControl);
            io.KeyAlt = keyboard.IsKeyDown(Keys.LeftAlt) || keyboard.IsKeyDown(Keys.RightAlt);
            io.KeySuper = keyboard.IsKeyDown(Keys.LeftWindows) || keyboard.IsKeyDown(Keys.RightWindows);

            io.DisplaySize = new System.Numerics.Vector2(_graphicsDevice.PresentationParameters.BackBufferWidth, _graphicsDevice.PresentationParameters.BackBufferHeight);
            io.DisplayFramebufferScale = new System.Numerics.Vector2(1f, 1f);

            io.MousePos = new System.Numerics.Vector2(mouse.X, mouse.Y);

            io.MouseDown[0] = mouse.LeftButton == ButtonState.Pressed;
            io.MouseDown[1] = mouse.RightButton == ButtonState.Pressed;
            io.MouseDown[2] = mouse.MiddleButton == ButtonState.Pressed;

            var scrollDelta = mouse.ScrollWheelValue - _scrollWheelValue;
            io.MouseWheel = scrollDelta > 0 ? 1 : scrollDelta < 0 ? -1 : 0;
            _scrollWheelValue = mouse.ScrollWheelValue;
        }

        #endregion Setup & Update

        #region Internals

        /// <summary>
        /// Gets the geometry as set up by ImGui and sends it to the graphics device
        /// </summary>
        private void RenderDrawData(ImDrawDataPtr drawData)
        {
            // Setup render state: alpha-blending enabled, no face culling, no depth testing, scissor enabled, vertex/texcoord/color pointers
            var lastViewport = _graphicsDevice.Viewport;
            var lastScissorBox = _graphicsDevice.ScissorRectangle;

            _graphicsDevice.BlendFactor = Color.White;
            _graphicsDevice.BlendState = BlendState.NonPremultiplied;
            _graphicsDevice.RasterizerState = _rasterizerState;
            _graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

            // Handle cases of screen coordinates != from framebuffer coordinates (e.g. retina displays)
            drawData.ScaleClipRects(ImGui.GetIO().DisplayFramebufferScale);

            // Setup projection
            _graphicsDevice.Viewport = new Viewport(0, 0, _graphicsDevice.PresentationParameters.BackBufferWidth, _graphicsDevice.PresentationParameters.BackBufferHeight);

            UpdateBuffers(drawData);

            RenderCommandLists(drawData);

            // Restore modified state
            _graphicsDevice.Viewport = lastViewport;
            _graphicsDevice.ScissorRectangle = lastScissorBox;
        }

        private unsafe void UpdateBuffers(ImDrawDataPtr drawData)
        {
            if (drawData.TotalVtxCount == 0)
            {
                return;
            }

            // Expand buffers if we need more room
            if (drawData.TotalVtxCount > _vertexBufferSize)
            {
                _vertexBuffer?.Dispose();

                _vertexBufferSize = (int)(drawData.TotalVtxCount * 1.5f);
                _vertexBuffer = new VertexBuffer(_graphicsDevice, DrawVertDeclaration.Declaration, _vertexBufferSize, BufferUsage.None);
                _vertexData = new byte[_vertexBufferSize * DrawVertDeclaration.Size];
            }

            if (_vertexBuffer is null || _vertexData is null)
            {
                throw new InvalidOperationException("Invalid buffer for guid.");
            }

            if (drawData.TotalIdxCount > _indexBufferSize)
            {
                _indexBuffer?.Dispose();

                _indexBufferSize = (int)(drawData.TotalIdxCount * 1.5f);
                _indexBuffer = new IndexBuffer(_graphicsDevice, IndexElementSize.SixteenBits, _indexBufferSize, BufferUsage.None);
                _indexData = new byte[_indexBufferSize * sizeof(ushort)];
            }

            if (_indexBuffer is null || _indexData is null)
            {
                throw new InvalidOperationException("Invalid buffer for guid.");
            }

            // Copy ImGui's vertices and indices to a set of managed byte arrays
            int vtxOffset = 0;
            int idxOffset = 0;

            for (int n = 0; n < drawData.CmdListsCount; n++)
            {
                ImDrawListPtr cmdList = drawData.CmdListsRange[n];

                fixed (void* vtxDstPtr = &_vertexData[vtxOffset * DrawVertDeclaration.Size])
                fixed (void* idxDstPtr = &_indexData[idxOffset * sizeof(ushort)])
                {
                    Buffer.MemoryCopy((void*)cmdList.VtxBuffer.Data, vtxDstPtr, _vertexData.Length, cmdList.VtxBuffer.Size * DrawVertDeclaration.Size);
                    Buffer.MemoryCopy((void*)cmdList.IdxBuffer.Data, idxDstPtr, _indexData.Length, cmdList.IdxBuffer.Size * sizeof(ushort));
                }

                vtxOffset += cmdList.VtxBuffer.Size;
                idxOffset += cmdList.IdxBuffer.Size;
            }

            // Copy the managed byte arrays to the gpu vertex- and index buffers
            _vertexBuffer.SetData(_vertexData, 0, drawData.TotalVtxCount * DrawVertDeclaration.Size);
            _indexBuffer.SetData(_indexData, 0, drawData.TotalIdxCount * sizeof(ushort));
        }

        private unsafe void RenderCommandLists(ImDrawDataPtr drawData)
        {
            _graphicsDevice.SetVertexBuffer(_vertexBuffer);
            _graphicsDevice.Indices = _indexBuffer;

            int vtxOffset = 0;
            int idxOffset = 0;

            for (int n = 0; n < drawData.CmdListsCount; n++)
            {
                ImDrawListPtr cmdList = drawData.CmdListsRange[n];

                for (int cmdi = 0; cmdi < cmdList.CmdBuffer.Size; cmdi++)
                {
                    ImDrawCmdPtr drawCmd = cmdList.CmdBuffer[cmdi];

                    if (!_loadedTextures.ContainsKey(drawCmd.TextureId))
                    {
                        throw new InvalidOperationException($"Could not find a texture with id '{drawCmd.TextureId}', please check your bindings");
                    }

                    // I don't really understand this? Why do we have an element count of zero?
                    // Since this crashes MonoGame, we will just skip it.
                    if (drawCmd.ElemCount == 0)
                    {
                        continue;
                    }

                    _graphicsDevice.ScissorRectangle = new Rectangle(
                        (int)drawCmd.ClipRect.X,
                        (int)drawCmd.ClipRect.Y,
                        (int)(drawCmd.ClipRect.Z - drawCmd.ClipRect.X),
                        (int)(drawCmd.ClipRect.W - drawCmd.ClipRect.Y)
                    );

                    var effect = UpdateEffect(_loadedTextures[drawCmd.TextureId]);

                    foreach (var pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

#pragma warning disable CS0618 // // FNA does not expose an alternative method.
                        _graphicsDevice.DrawIndexedPrimitives(
                            primitiveType: PrimitiveType.TriangleList,
                            baseVertex: vtxOffset,
                            minVertexIndex: 0,
                            numVertices: cmdList.VtxBuffer.Size,
                            startIndex: idxOffset,
                            primitiveCount: (int)drawCmd.ElemCount / 3
                        );
#pragma warning restore CS0618
                    }

                    idxOffset += (int)drawCmd.ElemCount;
                }

                vtxOffset += cmdList.VtxBuffer.Size;
            }
        }

        #endregion Internals

        public void Dispose()
        {
            foreach ((_, Texture2D texture) in _loadedTextures)
            {
                texture?.Dispose();
            }

            ImGui.SaveIniSettingsToDisk(Path.Join(GameDataManager.SaveBasePath, "imgui.ini"));
        }

        internal void InitTheme()
        {
            ImGui.StyleColorsDark();
            var dark = ImGui.GetStyle();
            
            var theme = Game.Profile.Theme;

            dark.FrameRounding = 3;
            dark.PopupRounding = 3;
            dark.WindowRounding = 6;
            dark.Colors[(int)ImGuiCol.Text] = theme.White;

            dark.Colors[(int)ImGuiCol.PopupBg] = theme.Bg;
            dark.Colors[(int)ImGuiCol.WindowBg] = theme.Bg;
            dark.Colors[(int)ImGuiCol.TitleBg] = theme.BgFaded;
            dark.Colors[(int)ImGuiCol.TitleBgActive] = theme.Faded;
            dark.Colors[(int)ImGuiCol.TextSelectedBg] = theme.Accent;
            dark.Colors[(int)ImGuiCol.ChildBg] = theme.Bg;
            dark.Colors[(int)ImGuiCol.PopupBg] = theme.Bg;
            dark.Colors[(int)ImGuiCol.Header] = theme.Faded;
            dark.Colors[(int)ImGuiCol.HeaderActive] = theme.Accent;
            dark.Colors[(int)ImGuiCol.HeaderHovered] = theme.Accent;
            dark.Colors[(int)ImGuiCol.TabActive] = theme.Accent;
            dark.Colors[(int)ImGuiCol.TabHovered] = theme.HighAccent;
            dark.Colors[(int)ImGuiCol.TabUnfocused] = theme.BgFaded;
            dark.Colors[(int)ImGuiCol.TabUnfocusedActive] = theme.HighAccent;
            dark.Colors[(int)ImGuiCol.Tab] = theme.BgFaded;
            dark.Colors[(int)ImGuiCol.DockingEmptyBg] = theme.BgFaded;
            dark.Colors[(int)ImGuiCol.DockingPreview] = theme.Faded;
            dark.Colors[(int)ImGuiCol.Button] = theme.Foreground;
            dark.Colors[(int)ImGuiCol.ButtonActive] = theme.HighAccent;
            dark.Colors[(int)ImGuiCol.ButtonHovered] = theme.Accent;
            dark.Colors[(int)ImGuiCol.FrameBg] = theme.BgFaded;
            dark.Colors[(int)ImGuiCol.FrameBgActive] = theme.Bg;
            dark.Colors[(int)ImGuiCol.FrameBgHovered] = theme.Bg;
            dark.Colors[(int)ImGuiCol.SeparatorActive] = theme.Accent;
            dark.Colors[(int)ImGuiCol.ButtonActive] = theme.HighAccent;

            ImGui.LoadIniSettingsFromDisk(Path.Join(GameDataManager.SaveBasePath, "imgui.ini"));
        }
    }
}