using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace CSCI3097_Game_Of_Life_3D
{
  /// <summary>
  /// This is the main type for your game
  /// </summary>
  public class GameOfLife : Microsoft.Xna.Framework.Game
  {
    Texture2D texture;
    GraphicsDeviceManager graphics;
    SpriteBatch spriteBatch;
    Model model;
    DualTextureEffect dead;
    DualTextureEffect effect;
    BasicEffect bEffect;
    Cell current_cell;
    Grid west_grid;
    Grid east_grid;
    Grid north_grid;
    Grid south_grid;
    Grid center_grid;
    private bool is_playing;
    private bool buttons_locked;
    Vector3 camera_angle;

    public GameOfLife()
    {
      graphics = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";
      //set fps to 10
      this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 10.0f);
    }

    // Allows the game to perform any initialization it needs to before starting to run.
    // This is where it can query for any required services and load any non-graphic
    // related content.  Calling base.Initialize will enumerate through any components
    // and initialize them as well.
    protected override void Initialize()
    {
      this.camera_angle = new Vector3(2, 3, -5);
      int grid_width = GraphicsDevice.Viewport.Width / 4;
      int grid_height = GraphicsDevice.Viewport.Height / 4;
      //initially set to pause mode
      this.is_playing = false;
      //set up the grids
      this.north_grid = new Grid(this, 10, 10, GraphicsDevice.Viewport.Width/2 - grid_width/2,
         GraphicsDevice.Viewport.Height/2 - grid_height - (grid_height/2), grid_width,
         grid_height);
      this.center_grid = new Grid(this, 10, 10, GraphicsDevice.Viewport.Width / 2 - grid_width/2,
        GraphicsDevice.Viewport.Height / 2 - (grid_height / 2), grid_width,
        grid_height);
      this.south_grid = new Grid(this, 10, 10, GraphicsDevice.Viewport.Width / 2 - grid_width/2,
        GraphicsDevice.Viewport.Height/2 + (grid_height / 2), grid_width,
        grid_height);
      this.west_grid = new Grid(this, 10, 10, GraphicsDevice.Viewport.Width/2 - grid_width/2 - grid_width,
         GraphicsDevice.Viewport.Height/2 - (grid_height/2), grid_width,
         grid_height);
      this.east_grid = new Grid(this, 10, 10, GraphicsDevice.Viewport.Width/2 + grid_width/2,
        GraphicsDevice.Viewport.Height/2 - (grid_height/2), grid_width,
        grid_height);
      //Components.Add(this.north_grid);
      //Components.Add(this.center_grid);
      //Components.Add(this.south_grid);
      //Components.Add(this.west_grid);
      //Components.Add(this.east_grid);

      base.Initialize();
      //set the west grid as current
      //this.current_cell = this.west_grid.firstCell();
      //this.current_cell.selectCell();
    }

    // LoadContent will be called once per game and is the place to load
    // all of your content.
    protected override void LoadContent()
    {
      // Create a new SpriteBatch, which can be used to draw textures.
      spriteBatch = new SpriteBatch(GraphicsDevice);
      model = Content.Load<Model>("box");
      bEffect = new BasicEffect(GraphicsDevice);
      bEffect.Projection = Matrix.CreatePerspectiveFieldOfView(
        MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 1.0f, 50.0f);
      bEffect.View = Matrix.CreateLookAt(
        new Vector3(0, 42, 12), Vector3.Zero, Vector3.Up);
      bEffect.DiffuseColor = Color.DarkGray.ToVector3();
      bEffect.Texture = Content.Load<Texture2D>("Cell_Alive");

      //effect = new DualTextureEffect(GraphicsDevice);
      //effect.Projection = Matrix.CreatePerspectiveFieldOfView(
      //    MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 1.0f, 50.0f);
      //effect.View = Matrix.CreateLookAt(
      //    new Vector3(0, 42, 12), Vector3.Zero, Vector3.Up);
      //effect.DiffuseColor = Color.DarkGray.ToVector3();
      //effect.Texture = Content.Load<Texture2D>("Cell_Alive");
      //effect.Texture2 = new Texture2D(GraphicsDevice, 1, 1);
      //effect.Texture2.SetData(new Color[] { new Color(196, 196, 196, 255) });
      //effect.Texture2 = Content.Load<Texture2D>("Cell_Kill");

      // TODO: use this.Content to load your game content here
      Services.AddService(typeof(SpriteBatch), spriteBatch);
    }

    // UnloadContent will be called once per game and is the place to unload
    // all content.
    protected override void UnloadContent()
    {
      // TODO: Unload any non ContentManager content here
    }

    // Allows the game to run logic such as updating the world,
    // checking for collisions, gathering input, and playing audio.
    protected override void Update(GameTime gameTime)
    {
      // Allows the game to exit
      if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
        this.Exit();

      ////////////////////////////////////////////////////////////////////
      #region movement_UPDATE
      ///////////////////////////////////////////////////////////////
      //if running on a pc
#if WINDOWS
      KeyboardState currentKeyboardState = Keyboard.GetState();

      //if the left arrow key is pressed
      if (currentKeyboardState.IsKeyDown(Keys.Left))
      {
        //move the camera
        this.camera_angle.X -= 1;
        this.camera_angle.Z += 1;
        //move the cursor left
        //this.moveCursorWest();
      }
      //if the up array key is pressed
      if (currentKeyboardState.IsKeyDown(Keys.Up))
      {
        //move the camera
        this.camera_angle.Y -= 1;
        //move the cursor up
        //this.moveCursorNorth();
      }
      //if the right arrow key is pressed
      if (currentKeyboardState.IsKeyDown(Keys.Right))
      {
        this.camera_angle.X += 1;
        //move the cursor right
        //this.moveCursorEast();
      }
      //if the down arrow key is pressed
      if (currentKeyboardState.IsKeyDown(Keys.Down))
      {
        this.camera_angle.Y += 1;
        //move the cursor down
        //this.moveCursorSouth();
      }
      //if the Z key is pressed
      if (currentKeyboardState.IsKeyDown(Keys.Z))
      {
        //birth the cell
        this.current_cell.birthCell();
        //update the cell to trigger new state
        this.current_cell.update();
      }
      //if the X key is pressed
      if (currentKeyboardState.IsKeyDown(Keys.X))
      {
        //kill the cell
        this.current_cell.killCell();
        //update the cell to trigger new state
        this.current_cell.update();
      }
      //if the space key is pressed
      if (currentKeyboardState.IsKeyDown(Keys.Space))
      {
        this.advanceGeneration();
      }
#endif
      GamePadState currentGamePadState = GamePad.GetState(PlayerIndex.One);
      ///////////////////////////////////////////////////////////////
      #region thumbstick_UPDATE
      //////////////////////////////////////////////////////////
      //if the user moved the left thumbstick
      if (currentGamePadState.ThumbSticks.Left.X != 0
        || currentGamePadState.ThumbSticks.Left.Y != 0)
      {
        //if the thumbstick is pointing in the left direction
        if (currentGamePadState.ThumbSticks.Left.X < 0)
        {
          //move the cursor to the left
          this.moveCursorWest();
        }
        //if the thumbstick is pointing in the right direction
        else if (currentGamePadState.ThumbSticks.Left.X > 0)
        {
          //move the cursor to the right
          this.moveCursorEast();
        }
        //if the thumbstick is pointing down
        if (currentGamePadState.ThumbSticks.Left.Y < 0)
        {
          //move the cursor down
          this.moveCursorSouth();
        }
        //if the thumbstick is pointing up
        else if (currentGamePadState.ThumbSticks.Left.Y > 0)
        {
          //move the cursor up
          this.moveCursorNorth();
        }
      }
      //////////////////////////////////////////////////////////
      #endregion thumbstick_UPDATE
      ///////////////////////////////////////////////////////////////
      ///////////////////////////////////////////////////////////////
      #region dpad_UPDATE
      //////////////////////////////////////////////////////////
      //if there is no lock on the gamepad
      if (this.buttons_locked == true)
      {
        //if the user presses the left dpad
        if (currentGamePadState.DPad.Left == ButtonState.Pressed)
        {
          //move the cursor to the left
          this.moveCursorWest();
          //set the semaphore
          this.buttons_locked = false;
        }
        //or if the user presses the up dpad
        else if (currentGamePadState.DPad.Up == ButtonState.Pressed)
        {
          //move the cursor up
          this.moveCursorNorth();
          //set the semaphore
          this.buttons_locked = false;
        }
        //or if the user presses the right dpad
        else if (currentGamePadState.DPad.Right == ButtonState.Pressed)
        {
          //move the cursor right
          this.moveCursorEast();
          //set the semaphore
          this.buttons_locked = false;
        }
        else if (currentGamePadState.DPad.Down == ButtonState.Pressed)
        {
          //move the cursor down
          this.moveCursorSouth();
          //set the semaphore
          this.buttons_locked = false;
        }
      }
      //////////////////////////////////////////////////////////
      #endregion dpad_UPDATE
      ///////////////////////////////////////////////////////////////

      ///////////////////////////////////////////////////////////////
      #endregion movement_UPDATE
      ////////////////////////////////////////////////////////////////////

      ////////////////////////////////////////////////////////////////////
      #region buttons_UPDATE
      ///////////////////////////////////////////////////////////////

      ///////////////////////////////////////////////////////////////
      #region generation_UPDATE
      //////////////////////////////////////////////////////////
      //if the game is in play or the right trigger is held
      if (this.is_playing == true ||
        currentGamePadState.Triggers.Right != 0)
      {
        //go to the next generation
        this.advanceGeneration();
        //NOTE: This is continuous update as long as game is in play
        //       or the right trigger is held down
      }
      //if the user presses the Y button
      if (currentGamePadState.Buttons.Y == ButtonState.Pressed
        && this.buttons_locked == true)
      {
        //if game is paused
        if (this.is_playing == false)
        {
          //play game
          this.startPlay();
        }
        //otherwise, if game is playing
        else
        {
          //pause game
          this.pausePlay();
        }
      }
      //if the user presses the X button
      if (currentGamePadState.Buttons.X == ButtonState.Pressed &&
        this.buttons_locked == true)
      {
        //thumb is not at rest, button pressed
        this.buttons_locked = false;
        //go to the next generation
        this.advanceGeneration();

        //NOTE: Will only update once per button press
      }
      //////////////////////////////////////////////////////////
      #endregion generation_UPDATE
      ///////////////////////////////////////////////////////////////

      ///////////////////////////////////////////////////////////////
      #region cell_UPDATE
      //////////////////////////////////////////////////////////
      //if the user pushes the A button while not moving through the cells
      if (currentGamePadState.Buttons.A == ButtonState.Pressed
          && this.buttons_locked == true)
      {
        //birth the selected cell
        this.current_cell.birthCell();
        //update cell to trigger new state
        this.current_cell.update();
      }
      //if the user pushes the B button while not moving through the cells
      if (currentGamePadState.Buttons.B == ButtonState.Pressed
          && this.buttons_locked == true)
      {
        //kill the selected cell
        this.current_cell.killCell();
        //update cell to trigger new state
        this.current_cell.update();
      }
      //////////////////////////////////////////////////////////
      #endregion cell_UPDATE
      ///////////////////////////////////////////////////////////////

      ///////////////////////////////////////////////////////////////
      #endregion buttons_UPDATE
      ////////////////////////////////////////////////////////////////////

      //if the controller is not in uses
      if (!this.inUse(currentGamePadState))
      {
        //reset the ui state to the ready position
        this.buttons_locked = true;
      }

      base.Update(gameTime);
    }

    //////////////////////////////////////////////////////////////////////
    #region helper_methods_UPDATE
    /////////////////////////////////////////////////////////////////

    /*
     * Will return whether or not the controller
     * is in use by the player.
     * REQUIRE:  given.gamePadState != null
     * ENSURE:   if any button or joystick is
     *            being used and sending input
     *            to program,
     *            return true.
     *           otherwise,
     *            return false.
     */
    private bool inUse(GamePadState gamePadState)
    {
      bool result = true;

      if (gamePadState.ThumbSticks.Left.X == 0
        && gamePadState.ThumbSticks.Left.Y == 0
        && gamePadState.Buttons.LeftStick == ButtonState.Released
        && gamePadState.ThumbSticks.Right.X == 0
        && gamePadState.ThumbSticks.Right.Y == 0
        && gamePadState.Buttons.RightStick == ButtonState.Released
        && gamePadState.DPad.Left == ButtonState.Released
        && gamePadState.DPad.Up == ButtonState.Released
        && gamePadState.DPad.Right == ButtonState.Released
        && gamePadState.DPad.Down == ButtonState.Released
        && gamePadState.Buttons.A == ButtonState.Released
        && gamePadState.Buttons.B == ButtonState.Released
        && gamePadState.Buttons.X == ButtonState.Released
        && gamePadState.Buttons.Y == ButtonState.Released
        && gamePadState.Triggers.Left == 0
        && gamePadState.Triggers.Right == 0
        && gamePadState.Buttons.LeftShoulder == ButtonState.Released
        && gamePadState.Buttons.RightShoulder == ButtonState.Released)
      {
        result = false;
      }

      return result;
    }

    /*
     * Will advance the grids forward by one generation.
     * ENSURE:   if cell.is_alive == true
     *            if cell.neighbors() < 2
     *             kill cell
     *            if cell.neighbors() == 2|3
     *             birth cell
     *            if cell.neighbors() > 3
     *             kill cell
     *           if cell.is_alive == false
     *            && cell.neighbors() == 3
     *             birth cell
     */
    private void advanceGeneration()
    {
      //prime the cells in the grids
      this.west_grid.primeCells();
      this.east_grid.primeCells();
      //update the cells
      this.west_grid.advanceCells();
      this.east_grid.advanceCells();
    }

    /*
     * Will put this grid into play mode.
     * ENSURE:   this.is_playing == true
     */
    public void startPlay()
    {
      this.is_playing = true;
    }

    /*
     * Will put this grid into pause mode.
     * ENSURE:   this.is_playing == false
     */
    public void pausePlay()
    {
      this.is_playing = false;
    }

    /*
     * Will move the cursor from the
     * currently selected cell to the one
     * to the west if there is one.
     * REQUIRE:  old.current_cell.westCell() != null
     * ENSURE:   new.current_cell == old.current_cell.westCell()
     */
    private void moveCursorWest()
    {
      //save the old current in a temp
      Cell temp = this.current_cell;
      temp.deselectCell();
      //move to the new cell
      this.current_cell = temp.westCell();
      //select it
      this.current_cell.selectCell();
    }

    /*
     * Will move the cursor form the
     * currently selected cell to the one
     * to the north if there is one.
     * REQUIRE:  old.current_cell.northCell() != null
     * ENSURE:   new.current_cell == old.current_cell.northCell()
     */
    private void moveCursorNorth()
    {
      //save the old current in a temp
      Cell temp = this.current_cell;
      temp.deselectCell();
      //move to the new cell
      this.current_cell = temp.northCell();
      //select it
      this.current_cell.selectCell();
    }

    /*
     * Will move the cursor from the
     * currently selected cell to the one
     * to the east if there is one.
     * REQUIRE:  old.current_cell.eastCell() != null
     * ENSURE:   new.current_Cell == old.current_cell.eastCell()
     */
    private void moveCursorEast()
    {
      //save the old current in a temp
      Cell temp = this.current_cell;
      temp.deselectCell();
      //move to the new cell
      this.current_cell = temp.eastCell();
      //select it
      this.current_cell.selectCell();
    }

    /*
     * Will move the cursor from the
     * currently selected cell to the one
     * to the east if there is one.
     * REQUIRE:  old.current_cell.southCell() != null
     * ENSURE:   new.current_cell == old.current_cell.southCell()
     */
    private void moveCursorSouth()
    {
      //save the old current in a temp
      Cell temp = this.current_cell;
      temp.deselectCell();
      //move to the new cell
      this.current_cell = temp.southCell();
      //select it
      this.current_cell.selectCell();
    }
    /////////////////////////////////////////////////////////////////
    #endregion helper_methods_UPDATE
    //////////////////////////////////////////////////////////////////////


    // This is called when the game should draw itself.
    protected override void Draw(GameTime gameTime)
    {
      GraphicsDevice.Clear(Color.CornflowerBlue);

      DrawModel(model, Matrix.Identity, bEffect);
      //DrawModel(model, Matrix.Identity, effect);

      base.Draw(gameTime);
    }


    private void DrawModel(Model m, Matrix world, DualTextureEffect be)
    {
      foreach (ModelMesh mm in m.Meshes)
      {
        foreach (ModelMeshPart mmp in mm.MeshParts)
        {
          be.World = world;
          GraphicsDevice.SetVertexBuffer(mmp.VertexBuffer, mmp.VertexOffset);
          GraphicsDevice.Indices = mmp.IndexBuffer;
          be.CurrentTechnique.Passes[0].Apply();
          GraphicsDevice.DrawIndexedPrimitives(
              PrimitiveType.TriangleList, 0, 0,
              mmp.NumVertices, mmp.StartIndex, mmp.PrimitiveCount);
        }
      }
    }
  }
}
