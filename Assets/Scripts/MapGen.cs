using UnityEngine;
using System.Collections;

public class MapGen : MonoBehaviour {

	public Transform L;
	public Transform D;
	public Transform R;
	public Transform U;
	public Transform LR;
	public Transform UD;
	public Transform LD;
	public Transform RD;
	public Transform RU;
	public Transform LU;

	//public Transform Player;

	private const int iL = 1;
	private const int iD = 2;
	private const int iR = 3;
	private const int iU = 4;
	private const int iLR = 5;
	private const int iUD = 6;
	private const int iLD = 7;
	private const int iRD = 8;
	private const int iRU = 9;
	private const int iLU = 10;

	private const int RIGHT = 0;
	private const int UP = 1;
	private const int LEFT = 2;
	private const int DOWN = 3;

	private const int DEND = 1;
	private const int HALL = 2;
	private const int ERIG = 3;
	private const int ELEF = 4;
	private const int TEEE = 5;
	private const int JUNC = 6;

	private int ROWMAX = 50;
	private int COLMAX = 50;
	private int TILEMAX = 2500;
	private int WINPATH = 30;

	public float TILESIZE = 9.982f;

	private int[,] grid, branchGrid;
	private int tileCount = 0, posRow = 0, posCol = 0, numBran = 0, tileLast = 0, tileExit = 0, tileEntrance = 0, branchStart = 0;
	private int[] tileOrient, tileDir, tileType, tileRow, tileCol;
	private bool[] tileBran;

	private Tile[,] map, mapBranch;
	private int startRow, startCol, startOrient;

	// Use this for initialization
	void Start () {
		grid = new int[ROWMAX, COLMAX];
		tileOrient = new int[TILEMAX];
		tileDir = new int[TILEMAX];
		tileType = new int[TILEMAX];
		tileRow = new int[TILEMAX];
		tileCol = new int[TILEMAX];
		tileBran = new bool[TILEMAX];
		map = new Tile[ROWMAX, COLMAX];
		gridGen();
		draw ();
		//Instantiate (Player, new Vector3 (startRow * 10, startCol * 10, 0), Quaternion.identity);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void draw() {
		for (int i = 0; i < ROWMAX; i++) {
			for (int j = 0; j < COLMAX; j++) {
				if(map[i,j] != null) {
					Transform drawing = null;
					switch (map[i,j].getImage()) {
						case iL:
							drawing = L;
							break;
						case iD:
							drawing = D;
							break;
						case iR:
							drawing = R;
							break;
						case iU:
							drawing = U;
							break;
						case iLR:
							drawing = LR;
							break;
						case iUD:
							drawing = UD;
							break;
						case iLD:
							drawing = LD;
							break;
						case iRD:
							drawing = RD;
							break;
						case iRU:
							drawing = RU;
							break;
						case iLU:
							drawing = LU;
							break;
						default:
							break;
					}
					Instantiate (drawing, new Vector3 (map[i,j].getRow() * TILESIZE, map[i,j].getCol() * TILESIZE, 2), Quaternion.identity);
				}
			}
		}
	}

	public void reGen(){
		grid = new int[ROWMAX, COLMAX];
		tileOrient = new int[TILEMAX];
		tileDir = new int[TILEMAX];
		tileType = new int[TILEMAX];
		tileRow = new int[TILEMAX];
		tileCol = new int[TILEMAX];
		tileBran = new bool[TILEMAX];
		map = new Tile[ROWMAX, COLMAX];
		gridGen();
	}

	private void gridGen() {
		//grid [posRow = Random.Range (0, ROWMAX), posCol = Random.Range (0, COLMAX)] = 1;
		startRow = tileRow [1] = posRow = 25;
		startCol = tileCol [1] = posCol = 25;
		grid[posRow,posCol] = 1;
		tileDir [0] = -1;
		tileDir [1] = -1;
		tileType [1] = DEND;
		tileEntrance = 1;
		tileCount = 1;
		while ((tileCount < WINPATH) && canMove(false)) {
			int direction;
			bool moved = false;
			while (moved == false){
				if (canMove(direction = Random.Range(0, 5), false)) {
					tileCount++;
					move(direction, false);
					moved = true;
				}
			}
		}

		title(2, tileCount, startAdj ());

		endAdj (tileCount);

		translate (1, tileCount);
		tileLast = tileCount;

		for (int i = 0; i < tileLast; i++) {
			if (tileBran[i]) {
				//branchGen(i);
			}
		}
	}

	private bool canMove(bool branch) {
		return ((canMoveRight (branch)
			|| canMoveUp (branch)
			|| canMoveLeft (branch)
			|| canMoveDown (branch))
			&& (tileCount < TILEMAX - 3));
	}

	private bool canMove(int dir, bool branch) {
		bool ret;

		switch (dir) {
			case RIGHT:
				ret = canMoveRight(branch);
				break;
			case UP:
				ret = canMoveUp(branch);
				break;
			case LEFT:
				ret = canMoveLeft(branch);
				break;
			case DOWN:
				ret = canMoveDown(branch);
				break;
			default:
				ret = false;
				break;
		}

		return (ret && (tileCount < TILEMAX - 3));
	}

	private bool canMoveDown(bool branch) {
		bool ret = false;
		if (!branch) {
			if (posRow < ROWMAX - 1) {
				if (grid [posRow + 1, posCol] == 0) {
					ret = true;
				}
			}
		} else {
			if (posRow < ROWMAX - 1) {
				if (branchGrid[posRow + 1, posCol] == 0) {
					ret = true;
				}
			}
		}
		return ret;
	}

	private bool canMoveLeft(bool branch) {
		bool ret = false;
		if (!branch) {
			if (posCol > 0) {
				if (grid [posRow, posCol - 1] == 0) {
					ret = true;
				}
			}
		} else {
			if (posCol > 0) {
				if (branchGrid[posRow, posCol - 1] == 0) {
					ret = true;
				}
			}
		}
		return ret;
	}

	private bool canMoveUp(bool branch) {
		bool ret = false;
		if (!branch) {
			if (posRow > 0) {
				if (grid [posRow - 1, posCol] == 0) {
					ret = true;
				}
			}
		} else {
			if (posRow > 0) {
				if (branchGrid[posRow - 1, posCol] == 0) {
					ret = true;
				}
			}
		}
		return ret;
	}

	private bool canMoveRight(bool branch) {
		bool ret = false;
		if (!branch) {
			if (posCol < COLMAX - 1) {
				if (grid [posRow, posCol + 1] == 0) {
					ret = true;
				}
			}
		} else {
			if (posCol < COLMAX - 1) {
				if (branchGrid[posRow, posCol + 1] == 0) {
					ret = true;
				}
			}
		}
		return ret;
	}

	private void move(int dir, bool branch) {
		switch (dir) {
			case RIGHT:
				posCol++;
				break;
			case UP:
				posRow--;
				break;
			case LEFT:
				posCol--;
				break;
			case DOWN:
				posRow++;
				break;
			default:
				break;
		}
		tileRow [tileCount] = posRow;
		tileCol [tileCount] = posCol;
		tileDir [tileCount] = dir;
		if (!branch) {
			grid [posRow, posCol] = tileCount;
			int tip = Random.Range (0, 101);
			if ((tip  < 40) && (numBran < (Mathf.Min(ROWMAX, COLMAX) / 3))) {
				tileBran [tileCount] = true;
				numBran++;
			}
			if (Random.Range (0, 101) < 25) {
				//puzzle?
			}
		} else {
			branchGrid[posRow, posCol] = tileCount;
		}
	}

	private int startAdj() {
		switch (tileDir [2]) {
			case RIGHT:
				tileOrient[1] = LEFT;
				break;
			case UP:
				tileOrient[1] = DOWN;
				break;
			case LEFT:
				tileOrient[1] = RIGHT;
				break;
			case DOWN:
				tileOrient[1] = UP;
				break;
			default:
				break;
		}
		startOrient = tileDir [2];
		return tileDir [2];
	}

	private void endAdj(int tile) {
		switch (tileType [tile - 1]) {
			case HALL:
				tileOrient[tile] = tileOrient[tile - 1];
				break;
			case ELEF:
				tileOrient[tile] = oriInc(tileOrient[tile - 1]);
				break;
			case ERIG:
				tileOrient[tile] = oriDec(tileOrient[tile - 1]);
				break;
		}
		tileType [tile] = DEND;
		tileExit = tile;
	}

	private void title(int start, int end, int orient) {
		for (int i = start; i < end; i++) {
			if (tileDir[i] == tileDir[i + 1]) {
				tileType[i] = HALL;
				tileOrient[i] = orient;
			} else if (oriInc(tileDir[i]) == tileDir[i + 1]) {
				tileType[i] = ELEF;
				tileOrient[i] = orient;
				orient = oriInc(orient);
			} else if (oriDec(tileDir[i]) == tileDir[i + 1]) {
				tileType[i] = ERIG;
				tileOrient[i] = orient;
				orient = oriDec(orient);
			}
		}
	}

	private int oriInc(int orient) {
		int ret = -1;
		ret = orient + 1;
		ret %= 4;
		return ret;
	}

	private int oriDec(int orient) {
		int ret = -1;
		if (orient == 0) {
			ret = 3;
		} else {
			ret = orient - 1;
		}
		return ret;
	}

	private void translate(int start, int end) {
		for (int i = start; i <= end; i++) {
			switch (tileType[i]) {
				case DEND:
					DeadEnd temp = new DeadEnd(tileRow[i], tileCol[i], tileOrient[i] * 90);
					if (i == tileExit) {
						temp.setExit();
					} else if (i == 1) {
						temp.setEntrance();
					}
					map[tileRow[i], tileCol[i]] = temp;
					break;
				case HALL:
					map[tileRow[i], tileCol[i]] = new Hall(tileRow[i], tileCol[i], tileOrient[i] * 90);
					break;
				case ERIG:
					map[tileRow[i], tileCol[i]] = new ElbowRight(tileRow[i], tileCol[i], tileOrient[i] * 90);
					break;
				case ELEF:
					map[tileRow[i], tileCol[i]] = new ElbowLeft(tileRow[i], tileCol[i], tileOrient[i] * 90);
					break;
				case TEEE:
					map[tileRow[i], tileCol[i]] = new Tee(tileRow[i], tileCol[i], tileOrient[i] * 90);
					break;
				case JUNC:
					map[tileRow[i], tileCol[i]] = new Junction(tileRow[i], tileCol[i], tileOrient[i] * 90);
					break;
				default:
					break;
			}
		}
	}

	private void branchGen(int i) {
		int branchLength = Mathf.Min (ROWMAX, COLMAX), thisBranch = 0;
		int direction = -1;
		branchStart = i;
		branchGrid = new int[ROWMAX, COLMAX];
		mapBranch = new Tile[ROWMAX, COLMAX];
		branchGrid [tileRow [tileEntrance], tileCol [tileEntrance]] = 1;
		branchGrid [tileRow [tileExit], tileCol [tileExit]] = 1;
		branchGrid [posRow = tileRow [i], posCol = tileCol [i]] = 1;
		
		for (int j = 0; j < branchLength; j++) {
			if (canMove (true)) {
				bool moved = false;
				while (moved == false) {
					if (canMove (direction = Random.Range (0, 5), true)) {
						tileCount++;
						thisBranch++;
						move (direction, true);
						moved = true;
					}
				}
			}
		}

		title (tileLast + 1, tileLast + thisBranch, branchAdj ());

		branchedAdj (tileLast + thisBranch - 1);

		branchTranslate (tileLast + 1, tileLast + thisBranch - 1);

		tileLast += thisBranch;

		for (int j = 0; j < ROWMAX; j++) {
			for (int k = 0; k < COLMAX; k++) {
				if (map[j, k] != null) {
					map[j,k] = map[j,k].merge(mapBranch[j,k]);
				} else {
					map[j, k] = mapBranch[j,k];
				}
			}
		}
	}

	private int branchAdj() {
		switch (tileDir [tileLast + 2]) {
			case RIGHT:
				tileOrient[tileLast + 1] = LEFT;
				break;
			case UP:
				tileOrient[tileLast + 1] = DOWN;
				break;
			case LEFT:
				tileOrient[tileLast + 1] = RIGHT;
				break;
			case DOWN:
				tileOrient[tileLast + 1] = UP;
				break;
			default:
				break;
		}
		switch (tileDir [tileLast + 1]) {
			case RIGHT:
				tileOrient[branchStart] = LEFT;
				break;
			case UP:
				tileOrient[branchStart] = DOWN;
				break;
			case LEFT:
				tileOrient[branchStart] = RIGHT;
				break;
			case DOWN:
				tileOrient[branchStart] = UP;
				break;
			default:
				break;
		}
		return tileDir [tileLast + 1];
	}

	private void branchedAdj(int tile) {
		switch (tileType [tile - 1]) {
			case HALL:
				tileOrient[tile] = tileOrient[tile - 1];
				break;
			case ELEF:
				tileOrient[tile] = oriInc(tileOrient[tile - 1]);
				break;
			case ERIG:
				tileOrient[tile] = oriDec(tileOrient[tile - 1]);
				break;
		}
		tileType [tile] = DEND;
	}

	private void branchTranslate(int start, int end) {
		for (int i = start; i <= end; i++) {
			switch (tileType[i]) {
				case DEND:
					mapBranch[tileRow[i], tileCol[i]] = new DeadEnd(tileRow[i], tileCol[i], tileOrient[i] * 90);
					break;
				case HALL:
					mapBranch[tileRow[i], tileCol[i]] = new Hall(tileRow[i], tileCol[i], tileOrient[i] * 90);
					break;
				case ERIG:
					mapBranch[tileRow[i], tileCol[i]] = new ElbowRight(tileRow[i], tileCol[i], tileOrient[i] * 90);
					break;
				case ELEF:
					mapBranch[tileRow[i], tileCol[i]] = new ElbowLeft(tileRow[i], tileCol[i], tileOrient[i] * 90);
					break;
				case TEEE:
					mapBranch[tileRow[i], tileCol[i]] = new Tee(tileRow[i], tileCol[i], tileOrient[i] * 90);
					break;
				case JUNC:
					mapBranch[tileRow[i], tileCol[i]] = new Junction(tileRow[i], tileCol[i], tileOrient[i] * 90);
					break;
				default:
					break;
			}
		}
		mapBranch[tileRow[branchStart], tileCol[branchStart]] = new DeadEnd(tileRow[branchStart], tileCol[branchStart], (tileOrient[branchStart] * 90));
	}

	/*------------- Inner classes begin here -------------*/
	private abstract class Tile {
		public int orientation, locationRow, locationCol, tileType = 0;
		protected bool entrance = false, exit = false;
		public bool branch = false;

		public Tile(int row, int col, int orient){
			locationRow = row;
			locationCol = col;
			orientation = orient;
		}

		public int getRow(){
			return locationRow;
		}

		public int getCol(){
			return locationCol;
		}

		public int getOrient(){
			return orientation;
		}

		public int getTileType(){
			return tileType;
		}

		public abstract int getImage();

		public abstract Tile merge(Tile mergingGuy);

		public int oriDec(int orient, int amt) {
			int ret = orient - amt;
			if (ret < 0) {
				ret += 360;
			}
			return ret;
		}

		public int oriInc(int orient, int amt) {
			int ret = orient + amt;
			ret %= 360;
			return ret;
		}
	}

	private class DeadEnd : Tile {

		public DeadEnd(int row, int col, int orient) : base(row, col, orient) {
			tileType = 1;
		}

		public void setEntrance() {
			entrance = true;
		}

		public bool checkEntrance() {
			return entrance;
		}

		public void setExit() {
			exit = true;
		}

		public bool checkExit() {
			return exit;
		}

		public override int getImage() {
			int ret = 0;
			switch (orientation) {
				case 0:
					ret = iD;
					break;
				case 90:
					ret = iR;
					break;
				case 180:
					ret = iU;
					break;
				case 270:
					ret = iL;
					break;
				default:
					break;
			}
			return ret;
		}

		
		public override Tile merge(Tile mergingGuy){
			Tile ret = this;

			if (mergingGuy != null) {
				int normOrient = orientation;
				int mergedType = -1, mergedOrient = 0;
				
				switch (mergingGuy.getTileType()) {
					case 1:
						switch (oriDec(mergingGuy.getOrient(), normOrient)) {
							case 0:
								mergedType = 1;
								mergedOrient = 0;
								break;
							case 90:
								mergedType = 3;
								mergedOrient = 0;
								break;
							case 180:
								mergedType = 2;
								mergedOrient = 0;
								break;
							case 270:
								mergedType = 3;
								mergedOrient = 270;
								break;
							default:
								mergedType = -1;
								mergedOrient = 0;
								break;
						}
						break;
					case 2:
						switch (oriDec(mergingGuy.getOrient(), normOrient)) {
							case 0:
							case 180:
								mergedType = 2;
								mergedOrient = 0;
								break;
							case 90:
							case 270:
								mergedType = 5;
								mergedOrient = 0;
								break;
							default:
								mergedType = -1;
								mergedOrient = 0;
								break;
						}
						break;
					case 3:
						switch (oriDec(mergingGuy.getOrient(), normOrient)) {
							case 0:
								mergedType = 3;
								mergedOrient = 0;
								break;
							case 90:
								mergedType = 5;
								mergedOrient = 90;
								break;
							case 180:
								mergedType = 5;
								mergedOrient = 270;
								break;
							case 270:
								mergedType = 3;
								mergedOrient = 270;
								break;
							default:
								mergedType = -1;
								mergedOrient = 0;
								break;
						}
						break;
					case 4:
						switch (oriDec (mergingGuy.getOrient(), normOrient)) {
							case 0:
								mergedType = 4;
								mergedOrient = 0;
								break;
							case 90:
								mergedType = 4;
								mergedOrient = 90;
								break;
							case 180:
								mergedType = 5;
								mergedOrient = 90;
								break;
							case 270:
								mergedType = 5;
								mergedOrient = 270;
								break;
							default:
								mergedType = -1;
								mergedOrient = 0;
								break;
						}
						break;
					case 5:
						switch (oriDec(mergingGuy.getOrient(), normOrient)) {
							case 0:
								mergedType = 5;
								mergedOrient = 0;
								break;
							case 90:
								mergedType = 5;
								mergedOrient = 0;
								break;
							case 180:
								mergedType = 6;
								mergedOrient = 0;
								break;
							case 270:
								mergedType = 5;
								mergedOrient = 270;
								break;
							default:
								mergedType = -1;
								mergedOrient = 0;
								break;
						}
						break;
					case 6:
						mergedType = 6;
						break;
					default:
						mergedType = -1;
						mergedOrient = 0;
						break;
				}
				switch (mergedType) {
					case 1:
						ret = new DeadEnd(locationRow, locationCol, oriInc(mergedOrient, normOrient));
						break;
					case 2:
						ret = new Hall(locationRow, locationCol, oriInc(mergedOrient, normOrient));
						break;
					case 3:
						ret = new ElbowRight(locationRow, locationCol, oriInc(mergedOrient, normOrient));
						break;
					case 4:
						ret = new ElbowLeft(locationRow, locationCol, oriInc(mergedOrient, normOrient));
						break;
					case 5:
						ret = new Tee(locationRow, locationCol, oriInc(mergedOrient, normOrient));
						break;
					case 6:
						ret = new Junction(locationRow, locationCol, oriInc(mergedOrient, normOrient));
						break;
					default:
						ret = this;
						break;
				}
			}
			return ret;
		}
	}

	private class Hall : Tile {

		public Hall(int row, int col, int orient) : base(row, col, orient) {
			tileType = 2;
		}

		public override int getImage() {
			int ret = 0;
			switch (orientation) {
				case 0:
				case 180:
					ret = iUD;
					break;
				case 90:
				case 270:
					ret = iLR;
					break;
				default:
					break;
			}
			return ret;
		}

		public override Tile merge(Tile mergingGuy){
			Tile ret = this;

			if (mergingGuy != null) {
				int normOrient = orientation;
				int mergedType = -1, mergedOrient = 0;

				switch (mergingGuy.getTileType()) {
					case 1:
						switch (oriDec(mergingGuy.getOrient(), normOrient)) {
							case 0:
							case 180:
								mergedType = 2;
								mergedOrient = 0;
								break;
							case 90:
								mergedType = 5;
								mergedOrient = 90;
								break;
							case 270:
								mergedType = 5;
								mergedOrient = 270;
								break;
							default:
								mergedType = -1;
								mergedOrient = 0;
								break;
						}
						break;
					case 2:
						switch (oriDec(mergingGuy.getOrient(), normOrient)) {
							case 0:
							case 180:
								mergedType = 2;
								mergedOrient = 0;
								break;
							case 90:
							case 270:
								mergedType = 6;
								mergedOrient = 0;
								break;
							default:
								mergedType = -1;
								mergedOrient = 0;
								break;
						}
						break;
					case 3:
						switch (oriDec(mergingGuy.getOrient(), normOrient)) {
							case 0:
							case 90:
								mergedType = 5;
								mergedOrient = 90;
								break;
							case 180:
							case 270:
								mergedType = 5;
								mergedOrient = 270;
								break;
							default:
								mergedType = -1;
								mergedOrient = 0;
								break;
						}
						break;
					case 4:
						switch (oriDec (mergingGuy.getOrient(), normOrient)) {
							case 90:
							case 180:
								mergedType = 5;
								mergedOrient = 90;
								break;
							case 0:
							case 270:
								mergedType = 5;
								mergedOrient = 270;
								break;
							default:
								mergedType = -1;
								mergedOrient = 0;
								break;
						}
						break;
					case 5:
						switch (oriDec(mergingGuy.getOrient(), normOrient)) {
							case 90:
								mergedType = 5;
								mergedOrient = 90;
								break;
							case 0:
							case 180:
								mergedType = 6;
								mergedOrient = 0;
								break;
							case 270:
								mergedType = 5;
								mergedOrient = 270;
								break;
							default:
								mergedType = -1;
								mergedOrient = 0;
								break;
						}
						break;
					case 6:
						mergedType = 6;
						break;
					default:
						mergedType = -1;
						mergedOrient = 0;
						break;
				}
				switch (mergedType) {
					case 2:
						ret = new Hall(locationRow, locationCol, oriInc(mergedOrient, normOrient));
						break;
					case 3:
						ret = new ElbowRight(locationRow, locationCol, oriInc(mergedOrient, normOrient));
						break;
					case 4:
						ret = new ElbowLeft(locationRow, locationCol, oriInc(mergedOrient, normOrient));
						break;
					case 5:
						ret = new Tee(locationRow, locationCol, oriInc(mergedOrient, normOrient));
						break;
					case 6:
						ret = new Junction(locationRow, locationCol, oriInc(mergedOrient, normOrient));
						break;
					default:
						ret = this;
						break;
				}
			}
			return ret;
		}
	}

	private class ElbowRight : Tile {

		public ElbowRight(int row, int col, int orient) : base(row, col, orient) {
			tileType = 3;
		}

		public override int getImage() {
			int ret = 0;
			switch (orientation) {
				case 0:
					ret = iRD;
					break;
				case 90:
					ret = iRU;
					break;
				case 180:
					ret = iLU;
					break;
				case 270:
					ret = iLD;
					break;
				default:
					break;
			}
			return ret;
		}
		
		public override Tile merge(Tile mergingGuy){
			Tile ret = this;

			if (mergingGuy != null) {
				int normOrient = orientation;
				int mergedType = -1, mergedOrient = 0;
				
				switch (mergingGuy.getTileType()) {
					case 1:
						switch (oriDec(mergingGuy.getOrient(), normOrient)) {
							case 0:
							case 90:
								mergedType = 3;
								mergedOrient = 0;
								break;
							case 180:
								mergedType = 5;
								mergedOrient = 90;
								break;
							case 270:
								mergedType = 5;
								mergedOrient = 0;
								break;
							default:
								mergedType = -1;
								mergedOrient = 0;
								break;
						}
						break;
					case 2:
						switch (oriDec(mergingGuy.getOrient(), normOrient)) {
							case 0:
							case 180:
								mergedType = 5;
								mergedOrient = 90;
								break;
							case 90:
							case 270:
								mergedType = 5;
								mergedOrient = 0;
								break;
							default:
								mergedType = -1;
								mergedOrient = 0;
								break;
						}
						break;
					case 3:
						switch (oriDec(mergingGuy.getOrient(), normOrient)) {
							case 0:
								mergedType = 3;
								mergedOrient = 0;
								break;
							case 90:
								mergedType = 5;
								mergedOrient = 90;
								break;
							case 180:
								mergedType = 6;
								mergedOrient = 0;
								break;
							case 270:
								mergedType = 5;
								mergedOrient = 0;
								break;
							default:
								mergedType = -1;
								mergedOrient = 0;
								break;
						}
						break;
					case 4:
						switch (oriDec (mergingGuy.getOrient(), normOrient)) {
							case 0:
								mergedType = 5;
								mergedOrient = 0;
								break;
							case 90:
								mergedType = 3;
								mergedOrient = 0;
								break;
							case 180:
								mergedType = 5;
								mergedOrient = 90;
								break;
							case 270:
								mergedType = 6;
								mergedOrient = 0;
								break;
							default:
								mergedType = -1;
								mergedOrient = 0;
								break;
						}
						break;
					case 5:
						switch (oriDec(mergingGuy.getOrient(), normOrient)) {
							case 0:
								mergedType = 5;
								mergedOrient = 0;
								break;
							case 90:
								mergedType = 5;
								mergedOrient = 90;
								break;
							case 180:
							case 270:
								mergedType = 6;
								mergedOrient = 0;
								break;
							default:
								mergedType = -1;
								mergedOrient = 0;
								break;
						}
						break;
					case 6:
						mergedType = 6;
						break;
					default:
						mergedType = -1;
						mergedOrient = 0;
						break;
				}
				switch (mergedType) {
					case 2:
						ret = new Hall(locationRow, locationCol, oriInc(mergedOrient, normOrient));
						break;
					case 3:
						ret = new ElbowRight(locationRow, locationCol, oriInc(mergedOrient, normOrient));
						break;
					case 4:
						ret = new ElbowLeft(locationRow, locationCol, oriInc(mergedOrient, normOrient));
						break;
					case 5:
						ret = new Tee(locationRow, locationCol, oriInc(mergedOrient, normOrient));
						break;
					case 6:
						ret = new Junction(locationRow, locationCol, oriInc(mergedOrient, normOrient));
						break;
					default:
						ret = this;
						break;
				}
			}
			return ret;
		}
	}

	private class ElbowLeft : Tile {

		public ElbowLeft(int row, int col, int orient) : base(row, col, orient) {
			tileType = 4;
		}

		public override int getImage() {
			int ret = 0;
			switch (orientation) {
				case 0:
					ret = iLD;
					break;
				case 90:
					ret = iRD;
					break;
				case 180:
					ret = iRU;
					break;
				case 270:
					ret = iLU;
					break;
				default:
					break;
			}
			return ret;
		}
		
		public override Tile merge(Tile mergingGuy){
			Tile ret = this;

			if (mergingGuy != null) {
				int normOrient = orientation;
				int mergedType = -1, mergedOrient = 0;
				
				switch (mergingGuy.getTileType()) {
					case 1:
						switch (oriDec(mergingGuy.getOrient(), normOrient)) {
							case 90:
								mergedType = 5;
								mergedOrient = 0;
								break;
							case 180:
								mergedType = 5;
								mergedOrient = 270;
								break;
							case 0:
							case 270:
								mergedType = 4;
								mergedOrient = 0;
								break;
							default:
								mergedType = -1;
								mergedOrient = 0;
								break;
						}
						break;
					case 2:
						switch (oriDec(mergingGuy.getOrient(), normOrient)) {
							case 0:
							case 180:
								mergedType = 5;
								mergedOrient = 270;
								break;
							case 90:
							case 270:
								mergedType = 5;
								mergedOrient = 0;
								break;
							default:
								mergedType = -1;
								mergedOrient = 0;
								break;
						}
						break;
					case 3:
						switch (oriDec(mergingGuy.getOrient(), normOrient)) {
							case 0:
								mergedType = 5;
								mergedOrient = 0;
								break;
							case 90:
								mergedType = 6;
								mergedOrient = 0;
								break;
							case 180:
								mergedType = 5;
								mergedOrient = 270;
								break;
							case 270:
								mergedType = 4;
								mergedOrient = 0;
								break;
							default:
								mergedType = -1;
								mergedOrient = 0;
								break;
						}
						break;
					case 4:
						switch (oriDec (mergingGuy.getOrient(), normOrient)) {
							case 0:
								mergedType = 4;
								mergedOrient = 0;
								break;
							case 90:
								mergedType = 5;
								mergedOrient = 0;
								break;
							case 180:
								mergedType = 6;
								mergedOrient = 0;
								break;
							case 270:
								mergedType = 5;
								mergedOrient = 270;
								break;
							default:
								mergedType = -1;
								mergedOrient = 0;
								break;
						}
						break;
					case 5:
						switch (oriDec(mergingGuy.getOrient(), normOrient)) {
							case 0:
								mergedType = 5;
								mergedOrient = 0;
								break;
							case 90:
							case 180:
								mergedType = 6;
								mergedOrient = 90;
								break;
							case 270:
								mergedType = 5;
								mergedOrient = 270;
								break;
							default:
								mergedType = -1;
								mergedOrient = 0;
								break;
						}
						break;
					case 6:
						mergedType = 6;
						break;
					default:
						mergedType = -1;
						mergedOrient = 0;
						break;
				}
				switch (mergedType) {
					case 2:
						ret = new Hall(locationRow, locationCol, oriInc(mergedOrient, normOrient));
						break;
					case 3:
						ret = new ElbowRight(locationRow, locationCol, oriInc(mergedOrient, normOrient));
						break;
					case 4:
						ret = new ElbowLeft(locationRow, locationCol, oriInc(mergedOrient, normOrient));
						break;
					case 5:
						ret = new Tee(locationRow, locationCol, oriInc(mergedOrient, normOrient));
						break;
					case 6:
						ret = new Junction(locationRow, locationCol, oriInc(mergedOrient, normOrient));
						break;
					default:
						ret = this;
						break;
				}
			}
			return ret;
		}
	}

	private class Tee : Tile {

		public Tee(int row, int col, int orient) : base(row, col, orient) {
			tileType = 5;
		}

		public override int getImage() {
			return 0;
		}
		
		public override Tile merge(Tile mergingGuy){
			Tile ret = this;

			if (mergingGuy != null) {
				int normOrient = orientation;
				int mergedType = -1, mergedOrient = 0;
				
				switch (mergingGuy.getTileType()) {
					case 1:
						switch (oriDec(mergingGuy.getOrient(), normOrient)) {
							case 0:
							case 90:
							case 270:
								mergedType = 5;
								mergedOrient = 0;
								break;
							case 180:
								mergedType = 6;
								mergedOrient = 0;
								break;
							default:
								mergedType = -1;
								mergedOrient = 0;
								break;
						}
						break;
					case 2:
						switch (oriDec(mergingGuy.getOrient(), normOrient)) {
							case 0:
							case 180:
								mergedType = 6;
								mergedOrient = 0;
								break;
							case 90:
							case 270:
								mergedType = 5;
								mergedOrient = 0;
								break;
							default:
								mergedType = -1;
								mergedOrient = 0;
								break;
						}
						break;
					case 3:
						switch (oriDec(mergingGuy.getOrient(), normOrient)) {
							case 90:
							case 180:
								mergedType = 6;
								mergedOrient = 0;
								break;
							case 0:
							case 270:
								mergedType = 5;
								mergedOrient = 0;
								break;
							default:
								mergedType = -1;
								mergedOrient = 0;
								break;
						}
						break;
					case 4:
						switch (oriDec (mergingGuy.getOrient(), normOrient)) {
							case 0:
							case 90:
								mergedType = 5;
								mergedOrient = 0;
								break;
							case 180:
							case 270:
								mergedType = 6;
								mergedOrient = 0;
								break;
							default:
								mergedType = -1;
								mergedOrient = 0;
								break;
						}
						break;
					case 5:
						switch (oriDec(mergingGuy.getOrient(), normOrient)) {
							case 0:
								mergedType = 5;
								mergedOrient = 0;
								break;
							case 90:
							case 180:
							case 270:
								mergedType = 6;
								mergedOrient = 0;
								break;
							default:
								mergedType = -1;
								mergedOrient = 0;
								break;
						}
						break;
					case 6:
						mergedType = 6;
						break;
					default:
						mergedType = -1;
						mergedOrient = 0;
						break;
				}
				switch (mergedType) {
					case 2:
						ret = new Hall(locationRow, locationCol, oriInc(mergedOrient, normOrient));
						break;
					case 3:
						ret = new ElbowRight(locationRow, locationCol, oriInc(mergedOrient, normOrient));
						break;
					case 4:
						ret = new ElbowLeft(locationRow, locationCol, oriInc(mergedOrient, normOrient));
						break;
					case 5:
						ret = new Tee(locationRow, locationCol, oriInc(mergedOrient, normOrient));
						break;
					case 6:
						ret = new Junction(locationRow, locationCol, oriInc(mergedOrient, normOrient));
						break;
					default:
						ret = this;
						break;
				}
			}
			return ret;
		}
	}

	private class Junction : Tile {

		public Junction(int row, int col, int orient) : base(row, col, orient) {
			tileType = 6;
		}

		public override int getImage(){
			return 0;
		}

		public override Tile merge(Tile mergingGuy) {
			return this;
		}
	} 
}