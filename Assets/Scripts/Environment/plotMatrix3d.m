function plotMatrix3d(M)
% simple 3d plot of a Matrix

[x,y]=meshgrid(1:length(M(1,:)),1:length(M(:,1)));
mesh(x,y,M);
zlim([-20,20]);