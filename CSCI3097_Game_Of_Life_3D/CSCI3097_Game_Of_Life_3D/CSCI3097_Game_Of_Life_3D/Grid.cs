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
    #region queries
    //////////////////////////////////////////////////////////////////////
  
    //////////////////////////////////////////////////////////////////////
    #endregion queries
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
      //set up the grid of cells
      this.setUpGrid();
      //link this grid's west border to the east
      this.linkEasternlyTo(this);
      //link this grid's east border to the west
      this.linkWesternlyTo(this);
      //link this grid's north border to the south
      this.linkSouthernlyTo(this);
      //link this grid's south border to the north
      this.linkNorthernlyTo(this);

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
    //////////////////////////////////////////////////////////////////////
    #region helper_methods_INITIALIZE
    /////////////////////////////////////////////////////////////////
    /*
     * Will initialize and set up the grid of cells
     * for use. Each cell will be initialized and any
     * boarder cells will not have a link past itself.
     * ENSURE:   this.grid.any_cell != null &&
     *            this.grid.west_border_cells.west_cell == null &&
     *            this.grid.north_border_cells.north_cell == null &&
     *            this.grid.east_border_cells.east_cell == null &&
     *            this.grid.south_border_cells.south_cell == null
     */
    internal void setUpGrid()
    {
      //initialize the array of cells
      this.cells = new Cell[this.cells_horizontal, this.cells_vertical];
      //now initialize each cell in the array
      for (int i = 0; i < this.cells_horizontal; i++)
      {
        for (int j = 0; j < this.cells_vertical; j++)
        {
          cells[i, j] = new Cell();
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
            //set the east_cell to the next cell
            cells[i, j].setEastCell(cells[i + 1, j]);
          }
          //if the current cell is not on the west border
          if (i > 0 && i < this.cells_horizontal)
          {
            //set this' west_cell to the previous cell
            cells[i, j].setWestCell(cells[i - 1, j]);
          }
          //if the current cell is not on the north border
          if (j > 0 && j < this.cells_vertical)
          {
            //set this' north_cell to the above cell
            cells[i, j].setNorthCell(cells[i, j - 1]);
          }
          //if the current cell is not on the south border
          if (j >= 0 && j < this.cells_vertical - 1)
          {
            //set this' south_cell to the below cell
            cells[i, j].setSouthCell(cells[i, j + 1]);
          }
        }
      }
    }
    /////////////////////////////////////////////////////////////////
    #endregion helper_methods_INITIALIZE
    //////////////////////////////////////////////////////////////////////
    /*
     * Will return the first cell in the grid.
     * ENSURE:   return this.cells[0,0]
     */
    public Cell firstCell()
    {
      return this.cells[0, 0];
    }

    /*
     * Will cycle through all cells, setting their cell.up
     * command to either kill or birth them next round.
     * ENSURE:   all.cells.setUpdate(KillCell XOR BirthCell)
     */ 
    public void primeCells() {
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
          if (currentCell.isAlive() == true)
          {
            //if the cell has fewer than two neighbors
            if (cellNeighbors < 2)
            {
              //kill the cell
              currentCell.killCell();
            }
            //if the cell has 2 or 3 neighbors
            if (cellNeighbors == 2 || cellNeighbors == 3)
            {
              //keep the cell alive
              currentCell.birthCell();
            }
            //if the cell has more than three neighbors
            if (cellNeighbors > 3)
            {
              //kill the cell
              currentCell.killCell();
            }
          }
          //otherwise, if the cell is dead
          else
          {
            //if the cell has exactly 3 neighbors
            if (cellNeighbors == 3)
            {
              //birth the cell
              currentCell.birthCell();
            }
          }
        }
      }
    }

    /*
     * Will update all cells, running their cell.up command.
     * ENSURE:   all.cells.up() is executed
     * NOTE:  IN ALMOST EVERY OCCASION, PRIMECELLS()
     *        SHOULD BE CALLED PRIOR TO THIS METHOD
     */
    public void advanceCells()
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
     * Will link this grid easternly to the given grid.
     * REQUIRE:  this.grid.cells_vertical == given.grid.cells_vertical
     * ENSURE:   this.grid.west_border_cells.west_cells 
     *            == given.grid.east_border_cells
     *           && given.grid.east_border_cells.east_cells
     *            == this.grid.west_border_cells
     */
    public void linkEasternlyTo(Grid west_grid) {
      //loop through the grid of cells
      for (int j = 0; j < this.cells_vertical; j++) {
        //set this grid's west border to the west_grid's east border
        this.cells[0,j].setWestCell(
          west_grid.cells[west_grid.cells_horizontal-1,j]);
        //set the west_grid's east border to this grid's west border
        west_grid.cells[west_grid.cells_horizontal-1,j].setEastCell(
          this.cells[0,j]);
      }
    }

    /*
     * Will link this grid southernly to the given grid.
     * REQUIRE:  this.grid.cells_horizontal == given.grid.cells_horizontal
     * ENSURE:   this.grid.north_border_cells.north_cells
     *            == given.grid.south_border_cells
     *           && given.grid.south_border_cells.south_cells
     *            == this.grid.north_border_cells
     */
    public void linkSouthernlyTo(Grid north_grid)
    {
      //loop through the grid of cells
      for (int i = 0; i < this.cells_horizontal; i++)
      {
        //set this grid's north border to the north_grid's south border
        this.cells[i, 0].setNorthCell(
          north_grid.cells[i, north_grid.cells_vertical - 1]);
        //set the north_grid's south border to this grid's north border
        north_grid.cells[i, north_grid.cells_vertical - 1].setSouthCell(
          this.cells[i, 0]);
      }
    }

    /*
     * Will link this grid westernly to the given grid.
     * REQUIRE:  this.grid.cells_vertical == given.grid.cells_vertical
     * ENSURE:   this.grid.east_border_cells.east_cells
     *            == given.grid.west_border_cells
     *           && given.grid.west_border_cells.west_cells
     *            == this.grid.east_border_cells
     */
    public void linkWesternlyTo(Grid east_grid)
    {
      //loop through the grid of cells
      for (int j = 0; j < this.cells_vertical; j++)
      {
        //set this grid's east border to the east_grid's west border
        this.cells[this.cells_horizontal - 1, j].setEastCell(
          east_grid.cells[0, j]);
        //set the east_grid's west border to this grid's east border
        east_grid.cells[0, j].setWestCell(
          this.cells[this.cells_horizontal - 1, j]);
      }
    }

    /*
     * Will link this grid northernly to the given grid.
     * REQUIRE:  this.grid.cells_horizontal == given.grid.cells_horizontal
     * ENSURE:   this.grid.south_border_cells.south_cells
     *            == given.grid.north_border_cells
     *           && given.grid.north_border_cells.north_cells
     *            == this.grid.south_border_cells
     */
    public void linkNorthernlyTo(Grid south_grid)
    {
      //loop through the grid of cells
      for (int i = 0; i < this.cells_horizontal; i++)
      {
        //set this grid's south border to the south_grid's north border
        this.cells[i, this.cells_vertical - 1].setSouthCell(
          south_grid.cells[i, 0]);
        //set the south_grid's north border to this grid's south border
        south_grid.cells[i, 0].setNorthCell(
          this.cells[i, this.cells_vertical - 1]);
      }
    }

    //Allows the game component to update itself.
    public override void Update(GameTime gameTime)
    {
      //GamePadState currentGamePadState = GamePad.GetState(PlayerIndex.One);

      ////////////////////////////////////////////////////////////////////
      #region camera_UPDATE
      ///////////////////////////////////////////////////////////////
      ////if the left trigger is pressed
      //if (currentGamePadState.Triggers.Left != 0)
      //{
      //  //enable camera zoom-in/out mode
      //  //if the right thumbstick is up
      //  if (currentGamePadState.ThumbSticks.Right.Y > 0)
      //  {
      //    //zoom in
      //    this.scale.X = this.scale.X + 0.01f;
      //    this.scale.Y = this.scale.Y + 0.01f;
      //  }
      //  //or if the right thumbstick is down
      //  else if (currentGamePadState.ThumbSticks.Right.Y < 0)
      //  {
      //    //zoom out
      //    this.scale.X = this.scale.X - 0.01f;
      //    this.scale.Y = this.scale.Y - 0.01f;
      //  }
      //}
      ////otherwise, enable camera movement mode
      //else
      //{
      //  //if the right thumbstick is in the left direction
      //  if (currentGamePadState.ThumbSticks.Right.X < 0)
      //  {
      //    //move the camera to the left
      //    this.grid_viewport.X = this.grid_viewport.X
      //      - (int)(currentGamePadState.ThumbSticks.Right.X * 100);
      //  }
      //  //or if the right thumbstick is in the right direction
      //  else if (currentGamePadState.ThumbSticks.Right.X > 0)
      //  {
      //    //move the camera to the right
      //    this.grid_viewport.X = this.grid_viewport.X
      //      - (int)(currentGamePadState.ThumbSticks.Right.X * 100);
      //  }
      //  //if the right thumbstick is up
      //  if (currentGamePadState.ThumbSticks.Right.Y > 0)
      //  {
      //    //move the camera up
      //    this.grid_viewport.Y = this.grid_viewport.Y
      //      + (int)(currentGamePadState.ThumbSticks.Right.Y * 100);
      //  }
      //  //or if the right thumbstick is down
      //  else if (currentGamePadState.ThumbSticks.Right.Y < 0)
      //  {
      //    //move the camera down
      //    this.grid_viewport.Y = this.grid_viewport.Y
      //      + (int)(currentGamePadState.ThumbSticks.Right.Y * 100);
      //  }
      //}
      ///////////////////////////////////////////////////////////////
      #endregion camera_UPDATE
      ////////////////////////////////////////////////////////////////////

      base.Update(gameTime);
    }

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
          if (this.cells[i, j].isAlive() == true)
          {
            spriteBatch.Draw(cell_alive, draw_rectangle, Color.White);
          }
          //otherwise draw the background texture
          else
          {
            spriteBatch.Draw(grid_background, draw_rectangle, Color.White);
          }
          //if the cell is selected, draw the selected texture
          if (this.cells[i, j].isSelected() == true)
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
