function T=createFractalTerrain(tSize, startRandRange, H, T)
% function T=createFractalTerrain(size, H, <T>)
% create fractal terrain by midpoint displacement (diamond square algorithm)
% size must be (power of 2) +1, e.g. 257
% the algorithm is prepared for pre-initialization of terrainpoints,
% i.e. some first steps could have been preinitialized to give a basic
% shape.
% the terrain is created iteratively: for every level the diamond step is
% performed first for the full terrain, then the square step is performed.
% input: size of terrain, (must be (2^x)+1 )
%        startRandRange: defines the overall elevation. size/2 gives
%        natural images
%        roughness H (between 0.0 and 1.0): 0.9 is a natural value
%                  H=0: max. roughness
%        optional: terrain T: T can be a predefined terrain, i.e. every
%                  matrix entry ~= inf will NOT be altered. This allows for
%                  preshaped terrain building
% output: terrain T

% ------------------------------------------------------
% check parameters
if exist('T')
    tSize = length(T(1,:));
end
l=log(tSize-1)/log(2);
if nargin <3 | ...
   l ~= round(l) | ...
   H>1.0 | H<0.0

    fprintf('Invalid parameter(s). usage of function:\n')
    help createFractalTerrain;
    T=[];
    return;
end

% ------------------------------------------------------
% init terrain
global TR;
if ~exist('T')
    TR=zeros(tSize)+inf;
    TR(1,1)=0;TR(1,tSize)=0;TR(tSize,1)=0;TR(tSize,tSize)=0;
else
    TR = T;
end
tSize=tSize-1;
start=[1,1];
randRange = startRandRange;

% ------------------------------------------------------
%                      MAINLOOP
% ------------------------------------------------------
while tSize>1
    % perform diamond step for entire terrain
    diamondStep(tSize, randRange);

    % perform square step for entire terrain
    squareStep(tSize, randRange);
  
    % adjust parameters for next scale
    tSize = tSize/2;
    randRange = randRange* (1/(2^H));
end

% ------------------------------------------------------
T=TR;
clear global TR;
return;

% ======================================================
% LOCAL FUNCTIONS
% ======================================================

% ------------------------------------------------------
% DIAMONDSTEP
% ------------------------------------------------------
function diamondStep(tSize, randRange)
global TR;

sh = tSize/2;
maxIndex = length(TR(:,1));    % size of terrain
row=1+sh; col=1+sh;            % row, col are the indices
                               % of each square's centerpoint

while (row < maxIndex)
    while(col < maxIndex)
        % average heightvalue of 4 cornerpoints
        value = TR(row-sh,col-sh) + ...
                TR(row-sh,col+sh) + ...
                TR(row+sh,col-sh) + ...
                TR(row+sh,col+sh);
        value = value / 4;
        
        % displacement
        displacement = rand(1) * randRange - randRange/2;
        value = value + displacement;
        
        % set diamond-point (if not predefined)
        if TR(row,col)==inf
            TR(row,col) = value;
        end
        
        % next square in same row
        col = col + tSize;
    end
    
    % next row
    col = 1+sh;
    row = row + tSize;
end
return


% ------------------------------------------------------
% SQUARESTEP
% ------------------------------------------------------

function squareStep(tSize, randRange)
global TR;

sh = tSize/2;
maxIndex = length(TR(:,1));    % size of terrain
colStart = 1+sh;
row=1; col=colStart;           % row, col are the indices
                               % of each diamond's centerpoint
                                              
while (row <= maxIndex)
    while(col <= maxIndex)                            
        value = 0;
        nop = 4;                % number of points
        
        % the following cases handle the boundary points,
        % i.e. the incomplete diamonds
       
        % north
        if row > 1
            value = value+TR(row-sh,col);
        else
            nop = nop -1;
        end                    
        % east
        if col < maxIndex
            value = value+TR(row,col+sh);
        else
            nop = nop -1;
        end                    
        % south
        if row < maxIndex
            value = value+TR(row+sh,col);
        else
            nop = nop -1;
        end
        % west
        if col > 1
            value = value+TR(row,col-sh);
        else
            nop = nop -1;
        end

        % displacement
        displacement = rand(1) * randRange - randRange/2;
        value = value/nop + displacement;
        
        % set square point (if not predefined)
        if TR(row,col)==inf
            TR(row,col) = value;
        end
        
        % next diamond in same row
        col = col + sh;
    end
    
    % next row
    % the starting column alternates between 1 and sh
    if colStart == 1
        colStart = sh+1;
    else
        colStart = 1;
    end
    
    col = colStart;
    row = row + sh;
end
return