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


namespace CSCI3097_Game_Of_Life_2D
{
  /// <summary>
  /// This is a game component that implements IUpdateable.
  /// </summary>
  public class Grid : Microsoft.Xna.Framework.DrawableGameComponent
  {
    ///////////////////////////////////////////////////////////////////////////
    #region intance_variables
    //////////////////////////////////////////////////////////////////////
    private Texture2D cell_alive;
    private Texture2D cell_kill;
    private Texture2D cell_selected;
    private Texture2D grid_background;
    private Cell[,] cells;
    private Cell current_cell;
    private bool thumb_at_rest;
    private bool is_playing;
    private int cells_horizontal;
    private int cells_vertical;
    private Rectangle grid_viewport;
    private Vector2 scale;

    //////////////////////////////////////////////////////////////////////
    #endregion instance_variables
    ///////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////
    #region constructors
    //////////////////////////////////////////////////////////////////////
    /*
     * Will create a new instance of this grid with the given
     * reference to a game, width and height (in cells) and
     * a default grid viewport of the given game's
     * GraphicsDevice.Viewport x, y, width and height values.
     * REQUIRE:  given game, cells_horizontal and cells_vertical != null
     * ENSURE:   grid.game == given.game &&
     *            grid.cells_horizontal == given.cells_horizontal &&
     *            grid.cells_vertical == given.cells_vertical &&
     *            grid.grid_viewport.x == given.game.graphicsdevice.viewport.x &&
     *            grid.grid_viewport.y == given.game.graphicsdevice.viewport.y &&
     *            grid.grid_viewport.width == given.game.graphicsdevice.viewport.width &&
     *            grid.grid_viewport.height == given.game.graphicsdevice.viewport.height
     */
    public Grid(Game game, int cells_horizontal, int cells_vertical)
      : base(game)
    {
      this.cells_horizontal = cells_horizontal;
      this.cells_vertical = cells_vertical;
      this.grid_viewport = new Rectangle(game.GraphicsDevice.Viewport.X,
        game.GraphicsDevice.Viewport.Y,
        game.GraphicsDevice.Viewport.Width,
        game.GraphicsDevice.Viewport.Height);
    }

    /*
     * Will create a new instance of this grid with the given
     * regerence to a game, width and height (in cells),
     * and overall width and height (in pixels).
     * REQUIRE:  given game, cells_horizontal, cells_vertical,
     *            grid_width and grid_height != null
     * ENSURE:   grid.game == given.game &&
     *            grid.cells_horizontal == given.cells_horizontal &&
     *            grid.cells_vertical == given.cells_vertical &&
     *            grid.grid_viewport.x == given.grid_x &&
     *            grid.grid_viewport.y == given.grid_y &&
     *            grid.grid_viewport.width == given.grid_width &&
     *            grid.grid_viewport.height == given.grid_height
     */
    public Grid(Game game, int cells_horizontal, int cells_vertical,
      int grid_x, int grid_y,
      int grid_width, int grid_height)
      : base(game)
    {
      this.cells_horizontal = cells_horizontal;
      this.cells_vertical = cells_vertical;
      this.grid_viewport = new Rectangle(grid_x,
        grid_y, grid_width, grid_height);
    }
    //////////////////////////////////////////////////////////////////////
    #endregion constructors
    ///////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////
    #region inner_class_CELL
    //////////////////////////////////////////////////////////////////////
    internal class Cell
    {
      ////////////////////////////////////////////////////////////////////
      #region CELL_instance_variables
      ///////////////////////////////////////////////////////////////
      //used to determine state of this Cell
      internal bool isAlive;
      //used to determine if this cell is currently selected
      internal bool isSelected;
      //used to update this cell's state
      internal UpdateCell up;
      //the cell to the west of this
      internal Cell westCell;
      //the cell to the east of this
      internal Cell eastCell;
      //the cell to the south of this
      internal Cell southCell;
      //the cell to the north of this
      internal Cell northCell;
      ///////////////////////////////////////////////////////////////
      #endregion CELL_instance_variables
      ////////////////////////////////////////////////////////////////////

      ////////////////////////////////////////////////////////////////////
      #region CELL_contructor
      ///////////////////////////////////////////////////////////////
      /*
       * Will create a new instance of this Cell.
       * Ensure:   cell.isAlive == false &&
       *            cell.isSelected == false &&
       *            cell.westCell == null &&
       *            cell.eastCell == null &&
       *            cell.northCell == null &&
       *            cell.southCell == null
       */
      internal Cell()
      {
        this.isAlive = false;
        this.isSelected = false;
        this.westCell = null;
        this.eastCell = null;
        this.southCell = null;
        this.northCell = null;
      }
      ///////////////////////////////////////////////////////////////
      #endregion CELL_constructor
      ////////////////////////////////////////////////////////////////////

      ////////////////////////////////////////////////////////////////////
      #region CELL_queries
      ///////////////////////////////////////////////////////////////
      /*
       * Will return the number of neighbors of this cell.
       * Ensure:   returns the number of cells connected
       *            to this cell that are alive.
       */
      internal int neighbors()
      {
        int result = 0;
        //set the additional four neighbors
        Cell northwestcell = (this.westCell != null && this.westCell.northCell != null) ? this.westCell.northCell : null;
        Cell northeastcell = (this.eastCell != null && this.eastCell.northCell != null) ? this.eastCell.northCell : null;
        Cell southeastcell = (this.eastCell != null && this.eastCell.southCell != null) ? this.eastCell.southCell : null;
        Cell southwestcell = (this.westCell != null && this.westCell.southCell != null) ? this.westCell.southCell : null;

        //if the westcell is not null && it is alive
        if (this.westCell != null && this.westCell.isAlive == true)
        {
          //add it to the result
          result = result + 1;
        }
        //if the northwestcell is not null && it is alive
        if (northwestcell != null && northwestcell.isAlive == true)
        {
          //add it to the result
          result = result + 1;
        }
        //if the northcell is not null && it is alive
        if (this.northCell != null && this.northCell.isAlive == true)
        {
          //add it to the result
          result = result + 1;
        }
        //if the northeastcell is not null && it is alive
        if (northeastcell != null && northeastcell.isAlive == true)
        {
          //add it to the result
          result = result + 1;
        }
        //if the eastcell is not null && it is alive
        if (this.eastCell != null && this.eastCell.isAlive == true)
        {
          //add it to the result
          result = result + 1;
        }
        //if the southeastcell is not null && it is alive
        if (southeastcell != null && southeastcell.isAlive == true)
        {
          //add it to the result
          result = result + 1;
        }
        //if the southcell is not null && it is alive
        if (this.southCell != null && this.southCell.isAlive == true)
        {
          //add it to the result
          result = result + 1;
        }
        //if the southwestcell is not null && it is alive
        if (southwestcell != null && southwestcell.isAlive == true)
        {
          //add it to the result
          result = result + 1;
        }
        return result;
      }
      ///////////////////////////////////////////////////////////////
      #endregion CELL_queries
      ////////////////////////////////////////////////////////////////////

      ////////////////////////////////////////////////////////////////////
      #region CELL_commands
      ///////////////////////////////////////////////////////////////
      /*
       * Will update this Cell's state based
       * on the current up command method.
       * Ensure:   if this.up == KillCell()
       *             cell.isAlive == false
       *           if this.up == BirthCell()
       *             cell.isAlive == true
       */
      internal void update()
      {
        this.up(this);
      }

      /*
       * Will change this.up to the given command.
       * Require:  given command is KillCell or BirthCell
       * Ensure:   this.up == KillCell() ||
       *            this.up == BirthCell()
       */
      internal void setUpdate(UpdateCell command)
      {
        this.up = command;
      }
      ///////////////////////////////////////////////////////////////
      #endregion CELL_commands
      ////////////////////////////////////////////////////////////////////
    }
    //////////////////////////////////////////////////////////////////////
    #endregion inner_class_CELL
    ///////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////
    #region delegate_methods_CELL
    //////////////////////////////////////////////////////////////////////
    //create the deleagte to be used to pass the kill or born commands
    internal delegate void UpdateCell(Cell currentCell);

    /*
     * Will kill the given cell, setting
     * currentCell.isAlive to false.
     * Require:  currentCell != null
     * Ensure:   currentCell.isAlive == false
     */
    internal void KillCell(Cell currentCell)
    {
      currentCell.isAlive = false;
    }

    /*
     * Will birth the given cell, setting
     * currentCell.isAlive to true.
     * Require:  currentCell != null
     * Ensure:   currentCell.isAlive == true
     */
    internal void BirthCell(Cell currentCell)
    {
      currentCell.isAlive = true;
    }
    //////////////////////////////////////////////////////////////////////
    #endregion delegate_methods_CELL
    ///////////////////////////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////////////////////////
    #region commands
    //////////////////////////////////////////////////////////////////////
    protected override void LoadContent()
    {
      //load the Cell Alive texture to be used later
      this.cell_alive = Game.Content.Load<Texture2D>("Cell_Alive");
      //load the Cell Kill texture to be used later
      this.cell_kill = Game.Content.Load<Texture2D>("Cell_Kill");
      //load the Cell Selected texture to be used later
      this.cell_selected = Game.Content.Load<Texture2D>("Cell_Selected");
      //load the Grid Background texture to be used later
      this.grid_background = Game.Content.Load<Texture2D>("Grid_Background");
      base.LoadContent();
    }

    //Allows the game component to perform any initialization it needs to before starting
    //to run.  This is where it can query for any required services and load content.
    public override void Initialize()
    {
      //set the ui to the ready state
      this.thumb_at_rest = true;
      //set the playing state to false
      this.is_playing = false;
      //initialize the array of cells
      this.cells = new Cell[this.cells_horizontal, this.cells_vertical];
      //now initialize each cell in the array
      for (int i = 0; i < this.cells_horizontal; i++)
      {
        for (int j = 0; j < this.cells_vertical; j++)
        {
          cells[i, j] = new Cell();
          cells[i, j].setUpdate(KillCell);
        }
      }

      //now cycle through again, setting cell paths
      for (int i = 0; i < this.cells_horizontal; i++)
      {
        for (int j = 0; j < this.cells_vertical; j++)
        {
          //if the current cell is not on the east border
          if (i >= 0 && i < this.cells_horizontal - 1)
          {
            //set the eastCell to the next cell
            cells[i, j].eastCell = cells[i + 1, j];
          }
          //or if the cell is on the east border
          else
          {
            //set the eastCell to the west-border cell
            cells[i, j].eastCell = cells[0, j];
          }
          //if the current cell is not on the west border
          if (i > 0 && i < this.cells_horizontal)
          {
            //set this' westCell to the previous cell
            cells[i, j].westCell = cells[i - 1, j];
          }
          //or if the cell is on the west border
          else
          {
            //set the westCell to the east-border cell
            cells[i, j].westCell = cells[this.cells_horizontal - 1, j];
          }
          //if the current cell is not on the north border
          if (j > 0 && j < this.cells_vertical)
          {
            //set this' northCell to the above cell
            cells[i, j].northCell = cells[i, j - 1];
          }
          //or if the cell is on the north border
          else
          {
            //set the northCell to the south-border cell
            cells[i, j].northCell = cells[i, this.cells_vertical - 1];
          }
          //if the current cell is not on the south border
          if (j >= 0 && j < this.cells_vertical - 1)
          {
            //set this' southCell to the below cell
            cells[i, j].southCell = cells[i, j + 1];
          }
          //or if the cell is on the south border
          else
          {
            //set the southCell to the north-border cell
            cells[i, j].southCell = cells[i, 0];
          }
        }
      }
      //set the north-west cell as selected
      this.cells[0, 0].isSelected = true;
      //save the selected cell
      this.current_cell = this.cells[0, 0];

      base.Initialize();
      //NOTE: MUST HAPPEN HERE SO LOADCONTENT IS CALLED
      //set the scale
      //scale = viewport.width/height / cell_line.width/height
      this.scale = new Vector2(
        (float)this.grid_viewport.Width /
        (float)((this.cells_horizontal * this.cell_alive.Width)
        + (this.cells_horizontal * 1)),
        (float)this.grid_viewport.Height /
        (float)((this.cells_vertical * this.cell_alive.Height)
        + (this.cells_vertical * 1)));
    }

    
    //Allows the game component to update itself.
    public override void Update(GameTime gameTime)
    {
      GamePadState currentGamePadState = GamePad.GetState(PlayerIndex.One);

      ////////////////////////////////////////////////////////////////////
      #region camera_UPDATE
      ///////////////////////////////////////////////////////////////
      //if the left trigger is pressed
      if (currentGamePadState.Triggers.Left != 0)
      {
        //enable camera zoom-in/out mode
        //if the right thumbstick is up
        if (currentGamePadState.ThumbSticks.Right.Y > 0)
        {
          //zoom in
          this.scale.X = this.scale.X + 0.01f;
          this.scale.Y = this.scale.Y + 0.01f;
        }
        //or if the right thumbstick is down
        else if (currentGamePadState.ThumbSticks.Right.Y < 0)
        {
          //zoom out
          this.scale.X = this.scale.X - 0.01f;
          this.scale.Y = this.scale.Y - 0.01f;
        }
      }
      //otherwise, enable camera movement mode
      else
      {
        //if the right thumbstick is in the left direction
        if (currentGamePadState.ThumbSticks.Right.X < 0)
        {
          //move the camera to the left
          this.grid_viewport.X = this.grid_viewport.X
            - (int)(currentGamePadState.ThumbSticks.Right.X * 100);
        }
        //or if the right thumbstick is in the right direction
        else if (currentGamePadState.ThumbSticks.Right.X > 0)
        {
          //move the camera to the right
          this.grid_viewport.X = this.grid_viewport.X
            - (int)(currentGamePadState.ThumbSticks.Right.X * 100);
        }
        //if the right thumbstick is up
        if (currentGamePadState.ThumbSticks.Right.Y > 0)
        {
          //move the camera up
          this.grid_viewport.Y = this.grid_viewport.Y
            + (int)(currentGamePadState.ThumbSticks.Right.Y * 100);
        }
        //or if the right thumbstick is down
        else if (currentGamePadState.ThumbSticks.Right.Y < 0)
        {
          //move the camera down
          this.grid_viewport.Y = this.grid_viewport.Y
            + (int)(currentGamePadState.ThumbSticks.Right.Y * 100);
        }
      }
      ///////////////////////////////////////////////////////////////
      #endregion camera_UPDATE
      ////////////////////////////////////////////////////////////////////

      ////////////////////////////////////////////////////////////////////
      #region movement_UPDATE
      ///////////////////////////////////////////////////////////////

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
          this.moveLeft();
        }
        //if the thumbstick is pointing in the right direction
        else if (currentGamePadState.ThumbSticks.Left.X > 0)
        {
          //move the cursor to the right
          this.moveRight();
        }
        //if the thumbstick is pointing down
        if (currentGamePadState.ThumbSticks.Left.Y < 0)
        {
          //move the cursor down
          this.moveDown();
        }
        //if the thumbstick is pointing up
        else if (currentGamePadState.ThumbSticks.Left.Y > 0)
        {
          //move the cursor up
          this.moveUp();
        }
      }
      //////////////////////////////////////////////////////////
      #endregion thumbstick_UPDATE
      ///////////////////////////////////////////////////////////////

      ///////////////////////////////////////////////////////////////
      #region dpad_UPDATE
      //////////////////////////////////////////////////////////
      //if there is no lock on the gamepad
      if (this.thumb_at_rest == true)
      {
        //if the user presses the left dpad
        if (currentGamePadState.DPad.Left == ButtonState.Pressed)
        {
          //move the cursor to the left
          this.moveLeft();
          //set the semaphore
          this.thumb_at_rest = false;
        }
        //or if the user presses the up dpad
        else if (currentGamePadState.DPad.Up == ButtonState.Pressed)
        {
          //move the cursor up
          this.moveUp();
          //set the semaphore
          this.thumb_at_rest = false;
        }
        //or if the user presses the right dpad
        else if (currentGamePadState.DPad.Right == ButtonState.Pressed)
        {
          //move the cursor right
          this.moveRight();
          //set the semaphore
          this.thumb_at_rest = false;
        }
        else if (currentGamePadState.DPad.Down == ButtonState.Pressed)
        {
          //move the cursor down
          this.moveDown();
          //set the semaphore
          this.thumb_at_rest = false;
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
        //set the cells update
        this.setCellsUpdate();
        //update the cells
        this.updateCells();
        //NOTE: This is continuous update as long as game is in play
        //       or the right trigger is held down
      }
      //if the user presses the Y button
      if (currentGamePadState.Buttons.Y == ButtonState.Pressed
        && this.thumb_at_rest == true)
      {
        //set game to playing mode
        this.is_playing = !this.is_playing;
      }
      //if the user presses the X button
      if (currentGamePadState.Buttons.X == ButtonState.Pressed &&
        this.thumb_at_rest == true)
      {
        //thumb is not at rest, button pressed
        this.thumb_at_rest = false;
        //set the cells update
        this.setCellsUpdate();

        //update the cells
        this.updateCells();
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
          && this.thumb_at_rest == true)
      {
        //birth the selected cell
        this.current_cell.setUpdate(BirthCell);
        this.current_cell.update();
      }
      //if the user pushes the B button while not moving through the cells
      if (currentGamePadState.Buttons.B == ButtonState.Pressed
          && this.thumb_at_rest == true)
      {
        //kill the selected cell
        this.current_cell.setUpdate(KillCell);
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
        this.thumb_at_rest = true;
      }
      base.Update(gameTime);
    }

    /////////////////////////////////////////////////////////////////
    #region helper_methods_UPDATE
    ////////////////////////////////////////////////////////////
    /*
     * Will change the currently selected cell to
     * the one on the left of the currently selected
     * cell if one exits.  Otherwise, the currently
     * selected cell will stay just that.
     * Require:  the user moved the left thumbstick
     *            to the left.
     * Ensure:   currently_selected_cell.isWestOf(old_selected_cell) == true
     */
    internal void moveLeft()
    {
      //if the current cell is not on the west border
      if (this.current_cell.westCell != null)
      {
        //then make the move

        //save the selected cell
        Cell tempCell = this.current_cell;

        //make it not selected
        tempCell.isSelected = false;
        //make the cell to the west selected
        tempCell.westCell.isSelected = true;
        //save the new selected cell
        this.current_cell = tempCell.westCell;
      }
    }

    /*
     * Will change the currently selected cell to
     * the one on the right of the currently selected
     * cell if one exits.  Otherwise, the currently
     * selected cell will stay just that.
     * Require:  the user moved the left thumbstick
     *            to the right.
     * Ensure:   currently_selected_cell.isEastOf(old_selected_cell) == true
     */
    internal void moveRight()
    {
      //if the current cell is not on the east border
      if (this.current_cell.eastCell != null)
      {
        //then make the move
        //save the current cell
        Cell tempCell = this.current_cell;

        //make it not selected
        tempCell.isSelected = false;
        //make the cell to the west selected
        tempCell.eastCell.isSelected = true;
        //save the new selected cell
        this.current_cell = tempCell.eastCell;
      }
    }

    /*
     * Will change the currently selected cell to
     * the one above the currently selected
     * cell if one exits.  Otherwise, the currently
     * selected cell will stay just that.
     * Require:  the user moved the left thumbstick up.
     * Ensure:   currently_selected_cell.isNorthOf(old_selected_cell) == true
     */
    internal void moveUp()
    {
      //if the current cell is not on the north border
      if (this.current_cell.northCell != null)
      {
        //save the current cell
        Cell tempCell = this.current_cell;

        //make it not selected
        tempCell.isSelected = false;
        //make the cell to the west selected
        tempCell.northCell.isSelected = true;
        //save the new selected cell
        this.current_cell = tempCell.northCell;
      }
    }

    /*
     * Will change the currently selected cell to
     * the one below the currently selected
     * cell if one exits.  Otherwise, the currently
     * selected cell will stay just that.
     * Require:  the user moved the left thumbstick down.
     * Ensure:   currently_selected_cell.isSouthOf(old_selected_cell) == true
     */
    internal void moveDown()
    {
      //if the current cell is not on the south border
      if (this.current_cell.southCell != null)
      {
        //save the current cell
        Cell tempCell = this.current_cell;

        //make it not selected
        tempCell.isSelected = false;
        //make the cell to the west selected
        tempCell.southCell.isSelected = true;
        //save the new selected cell
        this.current_cell = tempCell.southCell;
      }
    }

    /*
     * Will cycle through all cells, setting their cell.up
     * command to either kill or birth them next round.
     * ENSURE:   all.cells.setUpdate(KillCell XOR BirthCell
     */
    internal void setCellsUpdate()
    {
      //cycle through the grid of cells and update their up methods
      for (int i = 0; i < this.cells_horizontal; i++)
      {
        for (int j = 0; j < this.cells_vertical; j++)
        {
          //save the current cell
          Cell currentCell = cells[i, j];
          //save the number of neighbors it has
          int cellNeighbors = currentCell.neighbors();

          //if the cell is alive
          if (currentCell.isAlive == true)
          {
            //if the cell has fewer than two neighbors
            if (cellNeighbors < 2)
            {
              //kill the cell
              currentCell.setUpdate(KillCell);
            }
            //if the cell has 2 or 3 neighbors
            if (cellNeighbors == 2 || cellNeighbors == 3)
            {
              //keep the cell alive
              currentCell.setUpdate(BirthCell);
            }
            //if the cell has more than three neighbors
            if (cellNeighbors > 3)
            {
              //kill the cell
              currentCell.setUpdate(KillCell);
            }
          }
          //otherwise, if the cell is dead
          else
          {
            //if the cell has exactly 3 neighbors
            if (cellNeighbors == 3)
            {
              //birth the cell
              currentCell.setUpdate(BirthCell);
            }
          }
        }
      }
    }

    /*
     * Will update all cells, running their cell.up command.
     * ENSURE:   all.cells.up() is executed
     */
    internal void updateCells()
    {
      //cycle through the grid of cells, executing their up methods
      for (int i = 0; i < this.cells_horizontal; i++)
      {
        for (int j = 0; j < this.cells_vertical; j++)
        {
          this.cells[i, j].update();
        }
      }
    }

    /*
     * Will birth the selected cell, setting isAlive
     * to true.
     * Ensure:   currently_selected_cell.isAlive == true
     */
    internal void birthCell()
    {
      this.current_cell.setUpdate(BirthCell);
    }

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
    internal bool inUse(GamePadState gamePadState)
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
     * Will return the cell that is currently selected.
     * Ensure:   returned cell.isSelected == true
     */
    internal Cell currentSelection()
    {
      //variable to be returned
      Cell currently_selected_cell = new Cell();

      //cycle through the cells until the end is reached
      for (int i = 0; i < this.cells_horizontal; i++)
      {
        for (int j = 0; j < this.cells_vertical; j++)
        {
          //if the current cell is the one selected
          if (cells[i, j].isSelected == true)
          {
            //save the cell
            currently_selected_cell = cells[i, j];
          }
        }
      }

      return currently_selected_cell;
    }
    ////////////////////////////////////////////////////////////
    #endregion helper_methods_UPDATE
    /////////////////////////////////////////////////////////////////

    public override void Draw(GameTime gameTime)
    {
      SpriteBatch spriteBatch = Game.Services.GetService(
                typeof(SpriteBatch)) as SpriteBatch;

      spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
      //cycle through the cell array, drawing them to the screen
      for (int i = 0; i < this.cells_horizontal; i++)
      {
        for (int j = 0; j < this.cells_vertical; j++)
        {
          //compute the draw rectangle here
          Rectangle draw_rectangle = new Rectangle(
            this.grid_viewport.X
            + ((int)(this.cell_alive.Width * this.scale.X) * i)
            + (1 * i),
            this.grid_viewport.Y
            + ((int)(this.cell_alive.Height * this.scale.Y) * j)
            + (1 * j),
            (int)(this.cell_alive.Width * this.scale.X),
            (int)(this.cell_alive.Height * this.scale.Y)
            );

          //if the cell is alive, draw the alive texture
          if (this.cells[i, j].isAlive == true)
          {
            spriteBatch.Draw(cell_alive, draw_rectangle, Color.White);
          }
          //otherwise draw the background texture
          else
          {
            spriteBatch.Draw(grid_background, draw_rectangle, Color.White);
          }
          //if the cell is selected, draw the selected texture
          if (this.cells[i, j].isSelected == true)
          {
            spriteBatch.Draw(cell_selected, draw_rectangle, Color.White);
          }
        }
      }
      spriteBatch.End();
      base.Draw(gameTime);
    }
    //////////////////////////////////////////////////////////////////////
    #endregion commands
    //////////////////////////////////////////////////////////////////////
  }
}
